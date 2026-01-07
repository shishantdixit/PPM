using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Nozzle;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class NozzlesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<NozzlesController> _logger;

    public NozzlesController(ApplicationDbContext dbContext, ILogger<NozzlesController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all nozzles for the current tenant (or all tenants for super admin)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? machineId = null, [FromQuery] bool? activeOnly = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            IQueryable<Nozzle> query = _dbContext.Nozzles;

            // Filter by tenant for regular users
            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(n => n.TenantId == tenantId.Value);
            }
            else if (!isSuperAdmin && !tenantId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            if (machineId.HasValue)
                query = query.Where(n => n.MachineId == machineId.Value);

            if (activeOnly.HasValue && activeOnly.Value)
                query = query.Where(n => n.IsActive);

            var nozzles = await query
                .Include(n => n.Machine)
                .Include(n => n.FuelType)
                .OrderBy(n => n.Machine!.MachineName)
                .ThenBy(n => n.NozzleNumber)
                .Select(n => new NozzleDto
                {
                    NozzleId = n.NozzleId,
                    TenantId = n.TenantId,
                    MachineId = n.MachineId,
                    MachineName = n.Machine!.MachineName,
                    MachineCode = n.Machine.MachineCode,
                    FuelTypeId = n.FuelTypeId,
                    FuelName = n.FuelType!.FuelName,
                    FuelCode = n.FuelType.FuelCode,
                    FuelUnit = n.FuelType.Unit,
                    NozzleNumber = n.NozzleNumber,
                    NozzleName = n.NozzleName,
                    CurrentMeterReading = n.CurrentMeterReading,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<NozzleDto>>.SuccessResponse(nozzles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching nozzles");
            return StatusCode(500, ApiResponse<List<NozzleDto>>.ErrorResponse("Failed to fetch nozzles"));
        }
    }

    /// <summary>
    /// Get nozzles for a specific machine
    /// </summary>
    [HttpGet("machine/{machineId}")]
    public async Task<IActionResult> GetByMachine(Guid machineId)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            // Verify machine belongs to tenant
            var machineExists = await _dbContext.Machines
                .AnyAsync(m => m.MachineId == machineId && m.TenantId == tenantId.Value);

            if (!machineExists)
                return NotFound(ApiResponse<List<NozzleDto>>.ErrorResponse("Machine not found"));

            var nozzles = await _dbContext.Nozzles
                .Where(n => n.MachineId == machineId && n.TenantId == tenantId.Value)
                .Include(n => n.Machine)
                .Include(n => n.FuelType)
                .OrderBy(n => n.NozzleNumber)
                .Select(n => new NozzleDto
                {
                    NozzleId = n.NozzleId,
                    TenantId = n.TenantId,
                    MachineId = n.MachineId,
                    MachineName = n.Machine!.MachineName,
                    MachineCode = n.Machine.MachineCode,
                    FuelTypeId = n.FuelTypeId,
                    FuelName = n.FuelType!.FuelName,
                    FuelCode = n.FuelType.FuelCode,
                    NozzleNumber = n.NozzleNumber,
                    NozzleName = n.NozzleName,
                    CurrentMeterReading = n.CurrentMeterReading,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<NozzleDto>>.SuccessResponse(nozzles));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching nozzles for machine {MachineId}", machineId);
            return StatusCode(500, ApiResponse<List<NozzleDto>>.ErrorResponse("Failed to fetch nozzles"));
        }
    }

    /// <summary>
    /// Get nozzle by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var nozzle = await _dbContext.Nozzles
                .Where(n => n.NozzleId == id && n.TenantId == tenantId.Value)
                .Include(n => n.Machine)
                .Include(n => n.FuelType)
                .Select(n => new NozzleDto
                {
                    NozzleId = n.NozzleId,
                    TenantId = n.TenantId,
                    MachineId = n.MachineId,
                    MachineName = n.Machine!.MachineName,
                    MachineCode = n.Machine.MachineCode,
                    FuelTypeId = n.FuelTypeId,
                    FuelName = n.FuelType!.FuelName,
                    FuelCode = n.FuelType.FuelCode,
                    NozzleNumber = n.NozzleNumber,
                    NozzleName = n.NozzleName,
                    CurrentMeterReading = n.CurrentMeterReading,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (nozzle == null)
                return NotFound(ApiResponse<NozzleDto>.ErrorResponse("Nozzle not found"));

            return Ok(ApiResponse<NozzleDto>.SuccessResponse(nozzle));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching nozzle {NozzleId}", id);
            return StatusCode(500, ApiResponse<NozzleDto>.ErrorResponse("Failed to fetch nozzle"));
        }
    }

    /// <summary>
    /// Create a new nozzle (Owner/Manager only)
    /// </summary>
    [HttpPost]
    [RequireManager]
    public async Task<IActionResult> Create([FromBody] CreateNozzleDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            // Verify machine belongs to tenant
            var machine = await _dbContext.Machines
                .FirstOrDefaultAsync(m => m.MachineId == dto.MachineId && m.TenantId == tenantId.Value);

            if (machine == null)
                return NotFound(ApiResponse<NozzleDto>.ErrorResponse("Machine not found"));

            // Verify fuel type belongs to tenant
            var fuelType = await _dbContext.FuelTypes
                .FirstOrDefaultAsync(ft => ft.FuelTypeId == dto.FuelTypeId && ft.TenantId == tenantId.Value);

            if (fuelType == null)
                return NotFound(ApiResponse<NozzleDto>.ErrorResponse("Fuel type not found"));

            // Check if nozzle number already exists for this machine
            var exists = await _dbContext.Nozzles
                .AnyAsync(n => n.MachineId == dto.MachineId && n.NozzleNumber == dto.NozzleNumber);

            if (exists)
                return BadRequest(ApiResponse<NozzleDto>.ErrorResponse($"Nozzle number '{dto.NozzleNumber}' already exists for this machine"));

            var nozzle = new Nozzle
            {
                NozzleId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                MachineId = dto.MachineId,
                FuelTypeId = dto.FuelTypeId,
                NozzleNumber = dto.NozzleNumber.ToUpper(),
                NozzleName = dto.NozzleName,
                CurrentMeterReading = dto.CurrentMeterReading,
                IsActive = true
            };

            _dbContext.Nozzles.Add(nozzle);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created nozzle {NozzleNumber} for machine {MachineId}", nozzle.NozzleNumber, dto.MachineId);

            var result = new NozzleDto
            {
                NozzleId = nozzle.NozzleId,
                TenantId = nozzle.TenantId,
                MachineId = nozzle.MachineId,
                MachineName = machine.MachineName,
                MachineCode = machine.MachineCode,
                FuelTypeId = nozzle.FuelTypeId,
                FuelName = fuelType.FuelName,
                FuelCode = fuelType.FuelCode,
                NozzleNumber = nozzle.NozzleNumber,
                NozzleName = nozzle.NozzleName,
                CurrentMeterReading = nozzle.CurrentMeterReading,
                IsActive = nozzle.IsActive,
                CreatedAt = nozzle.CreatedAt,
                UpdatedAt = nozzle.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = nozzle.NozzleId },
                ApiResponse<NozzleDto>.SuccessResponse(result, "Nozzle created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating nozzle");
            return StatusCode(500, ApiResponse<NozzleDto>.ErrorResponse("Failed to create nozzle"));
        }
    }

    /// <summary>
    /// Update nozzle (Owner/Manager only)
    /// </summary>
    [HttpPut("{id}")]
    [RequireManager]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNozzleDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var nozzle = await _dbContext.Nozzles
                .Include(n => n.Machine)
                .Include(n => n.FuelType)
                .FirstOrDefaultAsync(n => n.NozzleId == id && n.TenantId == tenantId.Value);

            if (nozzle == null)
                return NotFound(ApiResponse<NozzleDto>.ErrorResponse("Nozzle not found"));

            if (dto.NozzleName != null)
                nozzle.NozzleName = dto.NozzleName;

            if (dto.CurrentMeterReading.HasValue)
            {
                // Only allow increasing meter readings
                if (dto.CurrentMeterReading.Value < nozzle.CurrentMeterReading)
                    return BadRequest(ApiResponse<NozzleDto>.ErrorResponse("Meter reading cannot be decreased"));

                nozzle.CurrentMeterReading = dto.CurrentMeterReading.Value;
            }

            if (dto.IsActive.HasValue)
                nozzle.IsActive = dto.IsActive.Value;

            nozzle.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated nozzle {NozzleId}", id);

            var result = new NozzleDto
            {
                NozzleId = nozzle.NozzleId,
                TenantId = nozzle.TenantId,
                MachineId = nozzle.MachineId,
                MachineName = nozzle.Machine!.MachineName,
                MachineCode = nozzle.Machine.MachineCode,
                FuelTypeId = nozzle.FuelTypeId,
                FuelName = nozzle.FuelType!.FuelName,
                FuelCode = nozzle.FuelType.FuelCode,
                NozzleNumber = nozzle.NozzleNumber,
                NozzleName = nozzle.NozzleName,
                CurrentMeterReading = nozzle.CurrentMeterReading,
                IsActive = nozzle.IsActive,
                CreatedAt = nozzle.CreatedAt,
                UpdatedAt = nozzle.UpdatedAt
            };

            return Ok(ApiResponse<NozzleDto>.SuccessResponse(result, "Nozzle updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating nozzle {NozzleId}", id);
            return StatusCode(500, ApiResponse<NozzleDto>.ErrorResponse("Failed to update nozzle"));
        }
    }

    /// <summary>
    /// Delete nozzle (Owner only)
    /// </summary>
    [HttpDelete("{id}")]
    [RequireOwner]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var nozzle = await _dbContext.Nozzles
                .FirstOrDefaultAsync(n => n.NozzleId == id && n.TenantId == tenantId.Value);

            if (nozzle == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Nozzle not found"));

            // In future: Check if nozzle has active shifts or transaction history
            // For now, allow deletion

            _dbContext.Nozzles.Remove(nozzle);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted nozzle {NozzleId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Nozzle deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting nozzle {NozzleId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete nozzle"));
        }
    }
}
