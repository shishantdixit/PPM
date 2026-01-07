using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Fuel;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class FuelRatesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FuelRatesController> _logger;

    public FuelRatesController(ApplicationDbContext dbContext, ILogger<FuelRatesController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get current fuel rates for all fuel types (or all tenants for super admin)
    /// </summary>
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentRates()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            IQueryable<FuelRate> query = _dbContext.FuelRates.Where(fr => fr.EffectiveTo == null);

            // Filter by tenant for regular users
            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(fr => fr.TenantId == tenantId.Value);
            }
            else if (!isSuperAdmin && !tenantId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            var currentRates = await query
                .Include(fr => fr.FuelType)
                .Select(fr => new CurrentFuelRateDto
                {
                    FuelTypeId = fr.FuelTypeId,
                    FuelName = fr.FuelType!.FuelName,
                    FuelCode = fr.FuelType.FuelCode,
                    Unit = fr.FuelType.Unit,
                    CurrentRate = fr.Rate,
                    EffectiveFrom = fr.EffectiveFrom
                })
                .ToListAsync();

            return Ok(ApiResponse<List<CurrentFuelRateDto>>.SuccessResponse(currentRates));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current fuel rates");
            return StatusCode(500, ApiResponse<List<CurrentFuelRateDto>>.ErrorResponse("Failed to fetch current fuel rates"));
        }
    }

    /// <summary>
    /// Get rate history for a specific fuel type (or all tenants for super admin)
    /// </summary>
    [HttpGet("history/{fuelTypeId}")]
    public async Task<IActionResult> GetRateHistory(Guid fuelTypeId)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            IQueryable<FuelRate> query = _dbContext.FuelRates.Where(fr => fr.FuelTypeId == fuelTypeId);

            // Filter by tenant for regular users
            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(fr => fr.TenantId == tenantId.Value);
            }
            else if (!isSuperAdmin && !tenantId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            var history = await query
                .Include(fr => fr.FuelType)
                .Include(fr => fr.UpdatedByUser)
                .OrderByDescending(fr => fr.EffectiveFrom)
                .Select(fr => new FuelRateDto
                {
                    FuelRateId = fr.FuelRateId,
                    TenantId = fr.TenantId,
                    FuelTypeId = fr.FuelTypeId,
                    FuelName = fr.FuelType!.FuelName,
                    FuelCode = fr.FuelType.FuelCode,
                    Rate = fr.Rate,
                    EffectiveFrom = fr.EffectiveFrom,
                    EffectiveTo = fr.EffectiveTo,
                    UpdatedBy = fr.UpdatedBy,
                    UpdatedByName = fr.UpdatedByUser != null ? fr.UpdatedByUser.FullName : null,
                    CreatedAt = fr.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<FuelRateDto>>.SuccessResponse(history));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fuel rate history for {FuelTypeId}", fuelTypeId);
            return StatusCode(500, ApiResponse<List<FuelRateDto>>.ErrorResponse("Failed to fetch fuel rate history"));
        }
    }

    /// <summary>
    /// Create a new fuel rate (Owner only)
    /// This will close the current rate and create a new one
    /// </summary>
    [HttpPost]
    [RequireOwner]
    public async Task<IActionResult> CreateRate([FromBody] CreateFuelRateDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var userId = HttpContext.GetUserId();

            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            // Verify fuel type exists and belongs to tenant
            var fuelType = await _dbContext.FuelTypes
                .FirstOrDefaultAsync(ft => ft.FuelTypeId == dto.FuelTypeId && ft.TenantId == tenantId.Value);

            if (fuelType == null)
                return NotFound(ApiResponse<FuelRateDto>.ErrorResponse("Fuel type not found"));

            // Validate rate is positive
            if (dto.Rate <= 0)
                return BadRequest(ApiResponse<FuelRateDto>.ErrorResponse("Rate must be greater than 0"));

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Close the current rate if exists
                var currentRate = await _dbContext.FuelRates
                    .FirstOrDefaultAsync(fr => fr.FuelTypeId == dto.FuelTypeId && fr.EffectiveTo == null);

                if (currentRate != null)
                {
                    currentRate.EffectiveTo = dto.EffectiveFrom;
                    currentRate.UpdatedAt = DateTime.UtcNow;
                }

                // Create new rate
                var newRate = new FuelRate
                {
                    FuelRateId = Guid.NewGuid(),
                    TenantId = tenantId.Value,
                    FuelTypeId = dto.FuelTypeId,
                    Rate = dto.Rate,
                    EffectiveFrom = dto.EffectiveFrom,
                    EffectiveTo = null, // Current rate
                    UpdatedBy = userId
                };

                _dbContext.FuelRates.Add(newRate);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Created new fuel rate for {FuelTypeId}: {Rate} effective from {EffectiveFrom}",
                    dto.FuelTypeId, dto.Rate, dto.EffectiveFrom);

                // Reload to get navigation properties
                var result = await _dbContext.FuelRates
                    .Where(fr => fr.FuelRateId == newRate.FuelRateId)
                    .Include(fr => fr.FuelType)
                    .Include(fr => fr.UpdatedByUser)
                    .Select(fr => new FuelRateDto
                    {
                        FuelRateId = fr.FuelRateId,
                        TenantId = fr.TenantId,
                        FuelTypeId = fr.FuelTypeId,
                        FuelName = fr.FuelType!.FuelName,
                        FuelCode = fr.FuelType.FuelCode,
                        Rate = fr.Rate,
                        EffectiveFrom = fr.EffectiveFrom,
                        EffectiveTo = fr.EffectiveTo,
                        UpdatedBy = fr.UpdatedBy,
                        UpdatedByName = fr.UpdatedByUser != null ? fr.UpdatedByUser.FullName : null,
                        CreatedAt = fr.CreatedAt
                    })
                    .FirstAsync();

                return Ok(ApiResponse<FuelRateDto>.SuccessResponse(result, "Fuel rate updated successfully"));
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fuel rate");
            return StatusCode(500, ApiResponse<FuelRateDto>.ErrorResponse("Failed to create fuel rate"));
        }
    }

    /// <summary>
    /// Update an existing fuel rate (Owner only)
    /// Can only update future rates, not current or past rates
    /// </summary>
    [HttpPut("{id}")]
    [RequireOwner]
    public async Task<IActionResult> UpdateRate(Guid id, [FromBody] UpdateFuelRateDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var fuelRate = await _dbContext.FuelRates
                .FirstOrDefaultAsync(fr => fr.FuelRateId == id && fr.TenantId == tenantId.Value);

            if (fuelRate == null)
                return NotFound(ApiResponse<FuelRateDto>.ErrorResponse("Fuel rate not found"));

            // Only allow updating future rates
            if (fuelRate.EffectiveFrom <= DateTime.UtcNow)
                return BadRequest(ApiResponse<FuelRateDto>.ErrorResponse("Cannot update current or past fuel rates"));

            if (dto.Rate <= 0)
                return BadRequest(ApiResponse<FuelRateDto>.ErrorResponse("Rate must be greater than 0"));

            fuelRate.Rate = dto.Rate;
            fuelRate.EffectiveFrom = dto.EffectiveFrom;
            fuelRate.UpdatedAt = DateTime.UtcNow;
            fuelRate.UpdatedBy = HttpContext.GetUserId();

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated fuel rate {FuelRateId}", id);

            var result = await _dbContext.FuelRates
                .Where(fr => fr.FuelRateId == id)
                .Include(fr => fr.FuelType)
                .Include(fr => fr.UpdatedByUser)
                .Select(fr => new FuelRateDto
                {
                    FuelRateId = fr.FuelRateId,
                    TenantId = fr.TenantId,
                    FuelTypeId = fr.FuelTypeId,
                    FuelName = fr.FuelType!.FuelName,
                    FuelCode = fr.FuelType.FuelCode,
                    Rate = fr.Rate,
                    EffectiveFrom = fr.EffectiveFrom,
                    EffectiveTo = fr.EffectiveTo,
                    UpdatedBy = fr.UpdatedBy,
                    UpdatedByName = fr.UpdatedByUser != null ? fr.UpdatedByUser.FullName : null,
                    CreatedAt = fr.CreatedAt
                })
                .FirstAsync();

            return Ok(ApiResponse<FuelRateDto>.SuccessResponse(result, "Fuel rate updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fuel rate {FuelRateId}", id);
            return StatusCode(500, ApiResponse<FuelRateDto>.ErrorResponse("Failed to update fuel rate"));
        }
    }

    /// <summary>
    /// Delete a future fuel rate (Owner only)
    /// Cannot delete current or past rates
    /// </summary>
    [HttpDelete("{id}")]
    [RequireOwner]
    public async Task<IActionResult> DeleteRate(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var fuelRate = await _dbContext.FuelRates
                .FirstOrDefaultAsync(fr => fr.FuelRateId == id && fr.TenantId == tenantId.Value);

            if (fuelRate == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Fuel rate not found"));

            // Only allow deleting future rates
            if (fuelRate.EffectiveFrom <= DateTime.UtcNow)
                return BadRequest(ApiResponse<object>.ErrorResponse("Cannot delete current or past fuel rates"));

            _dbContext.FuelRates.Remove(fuelRate);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted fuel rate {FuelRateId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Fuel rate deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fuel rate {FuelRateId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete fuel rate"));
        }
    }
}
