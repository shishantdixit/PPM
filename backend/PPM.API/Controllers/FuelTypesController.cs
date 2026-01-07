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
public class FuelTypesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FuelTypesController> _logger;

    public FuelTypesController(ApplicationDbContext dbContext, ILogger<FuelTypesController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all fuel types for the current tenant (or all tenants for super admin)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            IQueryable<FuelType> query = _dbContext.FuelTypes;

            // Filter by tenant for regular users
            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(ft => ft.TenantId == tenantId.Value);
            }
            else if (!isSuperAdmin && !tenantId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            var fuelTypes = await query
                .OrderBy(ft => ft.FuelName)
                .Select(ft => new FuelTypeDto
                {
                    FuelTypeId = ft.FuelTypeId,
                    TenantId = ft.TenantId,
                    FuelName = ft.FuelName,
                    FuelCode = ft.FuelCode,
                    Unit = ft.Unit,
                    IsActive = ft.IsActive,
                    CreatedAt = ft.CreatedAt,
                    UpdatedAt = ft.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<FuelTypeDto>>.SuccessResponse(fuelTypes));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fuel types");
            return StatusCode(500, ApiResponse<List<FuelTypeDto>>.ErrorResponse("Failed to fetch fuel types"));
        }
    }

    /// <summary>
    /// Get fuel type by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var fuelType = await _dbContext.FuelTypes
                .Where(ft => ft.FuelTypeId == id && ft.TenantId == tenantId.Value)
                .Select(ft => new FuelTypeDto
                {
                    FuelTypeId = ft.FuelTypeId,
                    TenantId = ft.TenantId,
                    FuelName = ft.FuelName,
                    FuelCode = ft.FuelCode,
                    Unit = ft.Unit,
                    IsActive = ft.IsActive,
                    CreatedAt = ft.CreatedAt,
                    UpdatedAt = ft.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (fuelType == null)
                return NotFound(ApiResponse<FuelTypeDto>.ErrorResponse("Fuel type not found"));

            return Ok(ApiResponse<FuelTypeDto>.SuccessResponse(fuelType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fuel type {FuelTypeId}", id);
            return StatusCode(500, ApiResponse<FuelTypeDto>.ErrorResponse("Failed to fetch fuel type"));
        }
    }

    /// <summary>
    /// Create a new fuel type (Owner/Manager only)
    /// </summary>
    [HttpPost]
    [RequireManager]
    public async Task<IActionResult> Create([FromBody] CreateFuelTypeDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            if (!tenantId.HasValue)
            {
                if (isSuperAdmin)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Super admins cannot create resources. Please log in as a tenant user to perform this operation."));
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            // Check if fuel code already exists for this tenant
            var exists = await _dbContext.FuelTypes
                .AnyAsync(ft => ft.TenantId == tenantId.Value && ft.FuelCode == dto.FuelCode);

            if (exists)
                return BadRequest(ApiResponse<FuelTypeDto>.ErrorResponse($"Fuel code '{dto.FuelCode}' already exists"));

            var fuelType = new FuelType
            {
                FuelTypeId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                FuelName = dto.FuelName,
                FuelCode = dto.FuelCode.ToUpper(),
                Unit = dto.Unit,
                IsActive = true
            };

            _dbContext.FuelTypes.Add(fuelType);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created fuel type {FuelCode} for tenant {TenantId}", fuelType.FuelCode, tenantId.Value);

            var result = new FuelTypeDto
            {
                FuelTypeId = fuelType.FuelTypeId,
                TenantId = fuelType.TenantId,
                FuelName = fuelType.FuelName,
                FuelCode = fuelType.FuelCode,
                Unit = fuelType.Unit,
                IsActive = fuelType.IsActive,
                CreatedAt = fuelType.CreatedAt,
                UpdatedAt = fuelType.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = fuelType.FuelTypeId },
                ApiResponse<FuelTypeDto>.SuccessResponse(result, "Fuel type created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fuel type");
            return StatusCode(500, ApiResponse<FuelTypeDto>.ErrorResponse("Failed to create fuel type"));
        }
    }

    /// <summary>
    /// Update fuel type (Owner/Manager only)
    /// </summary>
    [HttpPut("{id}")]
    [RequireManager]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFuelTypeDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            if (!tenantId.HasValue)
            {
                if (isSuperAdmin)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Super admins cannot modify resources. Please log in as a tenant user to perform this operation."));
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            var fuelType = await _dbContext.FuelTypes
                .FirstOrDefaultAsync(ft => ft.FuelTypeId == id && ft.TenantId == tenantId.Value);

            if (fuelType == null)
                return NotFound(ApiResponse<FuelTypeDto>.ErrorResponse("Fuel type not found"));

            if (dto.FuelName != null)
                fuelType.FuelName = dto.FuelName;

            if (dto.Unit != null)
                fuelType.Unit = dto.Unit;

            if (dto.IsActive.HasValue)
                fuelType.IsActive = dto.IsActive.Value;

            fuelType.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated fuel type {FuelTypeId}", id);

            var result = new FuelTypeDto
            {
                FuelTypeId = fuelType.FuelTypeId,
                TenantId = fuelType.TenantId,
                FuelName = fuelType.FuelName,
                FuelCode = fuelType.FuelCode,
                Unit = fuelType.Unit,
                IsActive = fuelType.IsActive,
                CreatedAt = fuelType.CreatedAt,
                UpdatedAt = fuelType.UpdatedAt
            };

            return Ok(ApiResponse<FuelTypeDto>.SuccessResponse(result, "Fuel type updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fuel type {FuelTypeId}", id);
            return StatusCode(500, ApiResponse<FuelTypeDto>.ErrorResponse("Failed to update fuel type"));
        }
    }

    /// <summary>
    /// Delete fuel type (Owner only)
    /// </summary>
    [HttpDelete("{id}")]
    [RequireOwner]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            if (!tenantId.HasValue)
            {
                if (isSuperAdmin)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Super admins cannot delete resources. Please log in as a tenant user to perform this operation."));
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            var fuelType = await _dbContext.FuelTypes
                .FirstOrDefaultAsync(ft => ft.FuelTypeId == id && ft.TenantId == tenantId.Value);

            if (fuelType == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Fuel type not found"));

            // Check if fuel type is being used by any nozzles
            var inUse = await _dbContext.Nozzles
                .AnyAsync(n => n.FuelTypeId == id);

            if (inUse)
                return BadRequest(ApiResponse<object>.ErrorResponse("Cannot delete fuel type that is assigned to nozzles"));

            _dbContext.FuelTypes.Remove(fuelType);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted fuel type {FuelTypeId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Fuel type deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fuel type {FuelTypeId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete fuel type"));
        }
    }
}
