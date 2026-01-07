using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Machine;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class MachinesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<MachinesController> _logger;

    public MachinesController(ApplicationDbContext dbContext, ILogger<MachinesController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all machines for the current tenant (or all tenants for super admin)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var isSuperAdmin = HttpContext.User.Claims.Any(c => c.Type == "IsSuperAdmin" && c.Value == "True");

            IQueryable<Machine> query = _dbContext.Machines;

            // Filter by tenant for regular users
            if (!isSuperAdmin && tenantId.HasValue)
            {
                query = query.Where(m => m.TenantId == tenantId.Value);
            }
            else if (!isSuperAdmin && !tenantId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));
            }

            if (activeOnly.HasValue && activeOnly.Value)
                query = query.Where(m => m.IsActive);

            var machines = await query
                .Include(m => m.Nozzles)
                .OrderBy(m => m.MachineName)
                .Select(m => new MachineDto
                {
                    MachineId = m.MachineId,
                    TenantId = m.TenantId,
                    MachineName = m.MachineName,
                    MachineCode = m.MachineCode,
                    SerialNumber = m.SerialNumber,
                    Manufacturer = m.Manufacturer,
                    Model = m.Model,
                    InstallationDate = m.InstallationDate,
                    Location = m.Location,
                    IsActive = m.IsActive,
                    NozzleCount = m.Nozzles.Count,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<MachineDto>>.SuccessResponse(machines));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching machines");
            return StatusCode(500, ApiResponse<List<MachineDto>>.ErrorResponse("Failed to fetch machines"));
        }
    }

    /// <summary>
    /// Get machine by ID with nozzles
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var machine = await _dbContext.Machines
                .Where(m => m.MachineId == id && m.TenantId == tenantId.Value)
                .Include(m => m.Nozzles)
                .Select(m => new MachineDto
                {
                    MachineId = m.MachineId,
                    TenantId = m.TenantId,
                    MachineName = m.MachineName,
                    MachineCode = m.MachineCode,
                    SerialNumber = m.SerialNumber,
                    Manufacturer = m.Manufacturer,
                    Model = m.Model,
                    InstallationDate = m.InstallationDate,
                    Location = m.Location,
                    IsActive = m.IsActive,
                    NozzleCount = m.Nozzles.Count,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (machine == null)
                return NotFound(ApiResponse<MachineDto>.ErrorResponse("Machine not found"));

            return Ok(ApiResponse<MachineDto>.SuccessResponse(machine));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching machine {MachineId}", id);
            return StatusCode(500, ApiResponse<MachineDto>.ErrorResponse("Failed to fetch machine"));
        }
    }

    /// <summary>
    /// Create a new machine (Owner/Manager only)
    /// </summary>
    [HttpPost]
    [RequireManager]
    public async Task<IActionResult> Create([FromBody] CreateMachineDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            // Check if machine code already exists for this tenant
            var exists = await _dbContext.Machines
                .AnyAsync(m => m.TenantId == tenantId.Value && m.MachineCode == dto.MachineCode);

            if (exists)
                return BadRequest(ApiResponse<MachineDto>.ErrorResponse($"Machine code '{dto.MachineCode}' already exists"));

            var machine = new Machine
            {
                MachineId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                MachineName = dto.MachineName,
                MachineCode = dto.MachineCode.ToUpper(),
                SerialNumber = dto.SerialNumber,
                Manufacturer = dto.Manufacturer,
                Model = dto.Model,
                InstallationDate = dto.InstallationDate,
                Location = dto.Location,
                IsActive = true
            };

            _dbContext.Machines.Add(machine);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created machine {MachineCode} for tenant {TenantId}", machine.MachineCode, tenantId.Value);

            var result = new MachineDto
            {
                MachineId = machine.MachineId,
                TenantId = machine.TenantId,
                MachineName = machine.MachineName,
                MachineCode = machine.MachineCode,
                SerialNumber = machine.SerialNumber,
                Manufacturer = machine.Manufacturer,
                Model = machine.Model,
                InstallationDate = machine.InstallationDate,
                Location = machine.Location,
                IsActive = machine.IsActive,
                NozzleCount = 0,
                CreatedAt = machine.CreatedAt,
                UpdatedAt = machine.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = machine.MachineId },
                ApiResponse<MachineDto>.SuccessResponse(result, "Machine created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating machine");
            return StatusCode(500, ApiResponse<MachineDto>.ErrorResponse("Failed to create machine"));
        }
    }

    /// <summary>
    /// Update machine (Owner/Manager only)
    /// </summary>
    [HttpPut("{id}")]
    [RequireManager]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMachineDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var machine = await _dbContext.Machines
                .Include(m => m.Nozzles)
                .FirstOrDefaultAsync(m => m.MachineId == id && m.TenantId == tenantId.Value);

            if (machine == null)
                return NotFound(ApiResponse<MachineDto>.ErrorResponse("Machine not found"));

            if (dto.MachineName != null)
                machine.MachineName = dto.MachineName;

            if (dto.SerialNumber != null)
                machine.SerialNumber = dto.SerialNumber;

            if (dto.Manufacturer != null)
                machine.Manufacturer = dto.Manufacturer;

            if (dto.Model != null)
                machine.Model = dto.Model;

            if (dto.InstallationDate.HasValue)
                machine.InstallationDate = dto.InstallationDate;

            if (dto.Location != null)
                machine.Location = dto.Location;

            if (dto.IsActive.HasValue)
                machine.IsActive = dto.IsActive.Value;

            machine.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated machine {MachineId}", id);

            var result = new MachineDto
            {
                MachineId = machine.MachineId,
                TenantId = machine.TenantId,
                MachineName = machine.MachineName,
                MachineCode = machine.MachineCode,
                SerialNumber = machine.SerialNumber,
                Manufacturer = machine.Manufacturer,
                Model = machine.Model,
                InstallationDate = machine.InstallationDate,
                Location = machine.Location,
                IsActive = machine.IsActive,
                NozzleCount = machine.Nozzles.Count,
                CreatedAt = machine.CreatedAt,
                UpdatedAt = machine.UpdatedAt
            };

            return Ok(ApiResponse<MachineDto>.SuccessResponse(result, "Machine updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating machine {MachineId}", id);
            return StatusCode(500, ApiResponse<MachineDto>.ErrorResponse("Failed to update machine"));
        }
    }

    /// <summary>
    /// Delete machine (Owner only)
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

            var machine = await _dbContext.Machines
                .Include(m => m.Nozzles)
                .FirstOrDefaultAsync(m => m.MachineId == id && m.TenantId == tenantId.Value);

            if (machine == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Machine not found"));

            // Check if machine has nozzles
            if (machine.Nozzles.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("Cannot delete machine with nozzles. Delete all nozzles first."));

            _dbContext.Machines.Remove(machine);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted machine {MachineId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Machine deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting machine {MachineId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete machine"));
        }
    }
}
