using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Shift;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class FuelSalesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FuelSalesController> _logger;

    public FuelSalesController(ApplicationDbContext dbContext, ILogger<FuelSalesController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Create a new fuel sale
    /// </summary>
    [HttpPost]
    [RequireWorker]
    public async Task<IActionResult> Create([FromBody] CreateFuelSaleDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse("Tenant context not found"));

            // Validate quantity
            if (dto.Quantity <= 0)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse("Quantity must be greater than zero"));

            // Fetch nozzle with tenant validation
            var nozzle = await _dbContext.Nozzles
                .Include(n => n.FuelType)
                .FirstOrDefaultAsync(n => n.NozzleId == dto.NozzleId && n.TenantId == tenantId.Value);

            if (nozzle == null)
                return NotFound(ApiResponse<FuelSaleDto>.ErrorResponse("Nozzle not found"));

            // Find active or pending shift containing this nozzle
            var shift = await _dbContext.Shifts
                .Include(s => s.NozzleReadings)
                .FirstOrDefaultAsync(s =>
                    s.TenantId == tenantId.Value
                    && (s.Status == ShiftStatus.Active || s.Status == ShiftStatus.Pending)
                    && s.NozzleReadings.Any(nr => nr.NozzleId == dto.NozzleId));

            if (shift == null)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse(
                    "No active or pending shift found for this nozzle. Please start a shift first."));

            if (shift.Status == ShiftStatus.Closed)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse(
                    "Cannot add sales to a closed shift"));

            // Get rate from shift's nozzle reading (locked at shift start)
            var shiftNozzleReading = shift.NozzleReadings
                .FirstOrDefault(nr => nr.NozzleId == dto.NozzleId);

            if (shiftNozzleReading == null)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse(
                    "Nozzle is not part of this shift"));

            var rate = shiftNozzleReading.RateAtShift;

            // Auto-calculate amount
            var amount = dto.Quantity * rate;

            // Create FuelSale entity
            var fuelSale = new FuelSale
            {
                FuelSaleId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                ShiftId = shift.ShiftId,
                NozzleId = dto.NozzleId,
                Quantity = dto.Quantity,
                Rate = rate,
                Amount = amount,
                PaymentMethod = dto.PaymentMethod,
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                VehicleNumber = dto.VehicleNumber,
                SaleTime = DateTime.UtcNow,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.FuelSales.Add(fuelSale);
            await _dbContext.SaveChangesAsync();

            // Return DTO with nozzle details
            var result = new FuelSaleDto
            {
                FuelSaleId = fuelSale.FuelSaleId,
                ShiftId = fuelSale.ShiftId,
                NozzleId = fuelSale.NozzleId,
                NozzleNumber = nozzle.NozzleNumber,
                FuelName = nozzle.FuelType!.FuelName,
                Quantity = fuelSale.Quantity,
                Rate = fuelSale.Rate,
                Amount = fuelSale.Amount,
                PaymentMethod = fuelSale.PaymentMethod,
                CustomerName = fuelSale.CustomerName,
                CustomerPhone = fuelSale.CustomerPhone,
                VehicleNumber = fuelSale.VehicleNumber,
                SaleTime = fuelSale.SaleTime,
                Notes = fuelSale.Notes
            };

            _logger.LogInformation(
                "Fuel sale {SaleId} recorded for shift {ShiftId} - {Quantity}L @ {Rate} = {Amount}",
                fuelSale.FuelSaleId, shift.ShiftId, fuelSale.Quantity, fuelSale.Rate, fuelSale.Amount);

            return CreatedAtAction(nameof(GetById), new { id = fuelSale.FuelSaleId },
                ApiResponse<FuelSaleDto>.SuccessResponse(result, "Sale recorded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fuel sale");
            return StatusCode(500, ApiResponse<FuelSaleDto>.ErrorResponse("Failed to record sale"));
        }
    }

    /// <summary>
    /// Get a specific fuel sale by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse("Tenant context not found"));

            var fuelSale = await _dbContext.FuelSales
                .Where(fs => fs.FuelSaleId == id && fs.TenantId == tenantId.Value)
                .Include(fs => fs.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .Select(fs => new FuelSaleDto
                {
                    FuelSaleId = fs.FuelSaleId,
                    ShiftId = fs.ShiftId,
                    NozzleId = fs.NozzleId,
                    NozzleNumber = fs.Nozzle!.NozzleNumber,
                    FuelName = fs.Nozzle.FuelType!.FuelName,
                    Quantity = fs.Quantity,
                    Rate = fs.Rate,
                    Amount = fs.Amount,
                    PaymentMethod = fs.PaymentMethod,
                    CustomerName = fs.CustomerName,
                    CustomerPhone = fs.CustomerPhone,
                    VehicleNumber = fs.VehicleNumber,
                    SaleTime = fs.SaleTime,
                    Notes = fs.Notes
                })
                .FirstOrDefaultAsync();

            if (fuelSale == null)
                return NotFound(ApiResponse<FuelSaleDto>.ErrorResponse("Sale not found"));

            return Ok(ApiResponse<FuelSaleDto>.SuccessResponse(fuelSale));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fuel sale {SaleId}", id);
            return StatusCode(500, ApiResponse<FuelSaleDto>.ErrorResponse("Failed to fetch sale"));
        }
    }

    /// <summary>
    /// Get all fuel sales for a specific shift
    /// </summary>
    [HttpGet("shift/{shiftId}")]
    public async Task<IActionResult> GetByShift(Guid shiftId)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<FuelSaleDto>>.ErrorResponse("Tenant context not found"));

            // Verify shift exists and belongs to tenant
            var shift = await _dbContext.Shifts
                .FirstOrDefaultAsync(s => s.ShiftId == shiftId && s.TenantId == tenantId.Value);

            if (shift == null)
                return NotFound(ApiResponse<List<FuelSaleDto>>.ErrorResponse("Shift not found"));

            // Fetch sales for the shift
            var sales = await _dbContext.FuelSales
                .Where(fs => fs.ShiftId == shiftId && fs.TenantId == tenantId.Value)
                .Include(fs => fs.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .OrderByDescending(fs => fs.SaleTime)
                .Select(fs => new FuelSaleDto
                {
                    FuelSaleId = fs.FuelSaleId,
                    ShiftId = fs.ShiftId,
                    NozzleId = fs.NozzleId,
                    NozzleNumber = fs.Nozzle!.NozzleNumber,
                    FuelName = fs.Nozzle.FuelType!.FuelName,
                    Quantity = fs.Quantity,
                    Rate = fs.Rate,
                    Amount = fs.Amount,
                    PaymentMethod = fs.PaymentMethod,
                    CustomerName = fs.CustomerName,
                    CustomerPhone = fs.CustomerPhone,
                    VehicleNumber = fs.VehicleNumber,
                    SaleTime = fs.SaleTime,
                    Notes = fs.Notes
                })
                .ToListAsync();

            return Ok(ApiResponse<List<FuelSaleDto>>.SuccessResponse(sales));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fuel sales for shift {ShiftId}", shiftId);
            return StatusCode(500, ApiResponse<List<FuelSaleDto>>.ErrorResponse("Failed to fetch sales"));
        }
    }

    /// <summary>
    /// Update an existing fuel sale
    /// </summary>
    [HttpPut("{id}")]
    [RequireWorker]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateFuelSaleDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse("Tenant context not found"));

            // Validate quantity
            if (dto.Quantity <= 0)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse("Quantity must be greater than zero"));

            // Fetch existing sale
            var fuelSale = await _dbContext.FuelSales
                .Include(fs => fs.Shift)
                .Include(fs => fs.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .FirstOrDefaultAsync(fs => fs.FuelSaleId == id && fs.TenantId == tenantId.Value);

            if (fuelSale == null)
                return NotFound(ApiResponse<FuelSaleDto>.ErrorResponse("Sale not found"));

            // Verify shift is not closed
            if (fuelSale.Shift!.Status == ShiftStatus.Closed)
                return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse(
                    "Cannot update sales for closed shifts"));

            // If nozzle changed, validate new nozzle and recalculate rate
            if (dto.NozzleId != fuelSale.NozzleId)
            {
                // Verify new nozzle exists and belongs to same shift
                var shiftNozzleReading = await _dbContext.ShiftNozzleReadings
                    .FirstOrDefaultAsync(snr =>
                        snr.ShiftId == fuelSale.ShiftId
                        && snr.NozzleId == dto.NozzleId
                        && snr.TenantId == tenantId.Value);

                if (shiftNozzleReading == null)
                    return BadRequest(ApiResponse<FuelSaleDto>.ErrorResponse(
                        "Selected nozzle is not part of this shift"));

                // Update nozzle and rate
                fuelSale.NozzleId = dto.NozzleId;
                fuelSale.Rate = shiftNozzleReading.RateAtShift;

                // Reload nozzle for response
                await _dbContext.Entry(fuelSale).Reference(fs => fs.Nozzle).LoadAsync();
                await _dbContext.Entry(fuelSale.Nozzle!).Reference(n => n.FuelType).LoadAsync();
            }

            // Update fields
            fuelSale.Quantity = dto.Quantity;
            fuelSale.Amount = fuelSale.Quantity * fuelSale.Rate; // Recalculate amount
            fuelSale.PaymentMethod = dto.PaymentMethod;
            fuelSale.CustomerName = dto.CustomerName;
            fuelSale.CustomerPhone = dto.CustomerPhone;
            fuelSale.VehicleNumber = dto.VehicleNumber;
            fuelSale.Notes = dto.Notes;
            fuelSale.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            // Return updated DTO
            var result = new FuelSaleDto
            {
                FuelSaleId = fuelSale.FuelSaleId,
                ShiftId = fuelSale.ShiftId,
                NozzleId = fuelSale.NozzleId,
                NozzleNumber = fuelSale.Nozzle!.NozzleNumber,
                FuelName = fuelSale.Nozzle.FuelType!.FuelName,
                Quantity = fuelSale.Quantity,
                Rate = fuelSale.Rate,
                Amount = fuelSale.Amount,
                PaymentMethod = fuelSale.PaymentMethod,
                CustomerName = fuelSale.CustomerName,
                CustomerPhone = fuelSale.CustomerPhone,
                VehicleNumber = fuelSale.VehicleNumber,
                SaleTime = fuelSale.SaleTime,
                Notes = fuelSale.Notes
            };

            _logger.LogInformation("Fuel sale {SaleId} updated", id);

            return Ok(ApiResponse<FuelSaleDto>.SuccessResponse(result, "Sale updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fuel sale {SaleId}", id);
            return StatusCode(500, ApiResponse<FuelSaleDto>.ErrorResponse("Failed to update sale"));
        }
    }

    /// <summary>
    /// Delete a fuel sale (Manager/Owner only)
    /// </summary>
    [HttpDelete("{id}")]
    [RequireManager]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            // Fetch sale with shift
            var fuelSale = await _dbContext.FuelSales
                .Include(fs => fs.Shift)
                .FirstOrDefaultAsync(fs => fs.FuelSaleId == id && fs.TenantId == tenantId.Value);

            if (fuelSale == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Sale not found"));

            // Verify shift is not closed
            if (fuelSale.Shift!.Status == ShiftStatus.Closed)
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Cannot delete sales from closed shifts"));

            _dbContext.FuelSales.Remove(fuelSale);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Fuel sale {SaleId} deleted from shift {ShiftId}",
                id, fuelSale.ShiftId);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Sale deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fuel sale {SaleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete sale"));
        }
    }
}
