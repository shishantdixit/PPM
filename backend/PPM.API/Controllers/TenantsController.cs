using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.Application.Common;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(ApplicationDbContext dbContext, ILogger<TenantsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all tenants with pagination (Super Admin only)
    /// </summary>
    [HttpGet]
    [RequireSuperAdmin]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null)
    {
        try
        {
            var query = _dbContext.Tenants.AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.SubscriptionStatus == status);
            }

            // Search by company name, owner name, or tenant code
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    t.CompanyName.Contains(search) ||
                    t.OwnerName.Contains(search) ||
                    t.TenantCode.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var tenants = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var response = new PagedResponse<Tenant>
            {
                Items = tenants,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };

            return Ok(ApiResponse<PagedResponse<Tenant>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenants");
            return StatusCode(500, ApiResponse<PagedResponse<Tenant>>.ErrorResponse("Failed to fetch tenants"));
        }
    }

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenant = await _dbContext.Tenants
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.TenantId == id);

            if (tenant == null)
            {
                return NotFound(ApiResponse<Tenant>.ErrorResponse("Tenant not found"));
            }

            return Ok(ApiResponse<Tenant>.SuccessResponse(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenant {TenantId}", id);
            return StatusCode(500, ApiResponse<Tenant>.ErrorResponse("Failed to fetch tenant"));
        }
    }

    /// <summary>
    /// Create new tenant (Super Admin only)
    /// </summary>
    [HttpPost]
    [RequireSuperAdmin]
    public async Task<IActionResult> Create([FromBody] CreateTenantDto dto)
    {
        try
        {
            // Check if tenant code already exists
            var existingTenant = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.TenantCode == dto.TenantCode);

            if (existingTenant != null)
            {
                return BadRequest(ApiResponse<Tenant>.ErrorResponse("Tenant code already exists"));
            }

            // Check if email already exists
            var existingEmail = await _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.Email == dto.Email);

            if (existingEmail != null)
            {
                return BadRequest(ApiResponse<Tenant>.ErrorResponse("Email already exists"));
            }

            var tenant = new Tenant
            {
                TenantId = Guid.NewGuid(),
                TenantCode = dto.TenantCode,
                CompanyName = dto.CompanyName,
                OwnerName = dto.OwnerName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country ?? "India",
                PinCode = dto.PinCode,
                SubscriptionPlan = dto.SubscriptionPlan ?? "Basic",
                SubscriptionStatus = "Active",
                SubscriptionStartDate = DateTime.UtcNow,
                SubscriptionEndDate = DateTime.UtcNow.AddYears(1),
                MaxMachines = dto.MaxMachines ?? 5,
                MaxWorkers = dto.MaxWorkers ?? 20,
                MaxMonthlyBills = dto.MaxMonthlyBills ?? 10000,
                IsActive = true
            };

            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created new tenant {TenantCode}", tenant.TenantCode);

            return CreatedAtAction(nameof(GetById), new { id = tenant.TenantId },
                ApiResponse<Tenant>.SuccessResponse(tenant, "Tenant created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant");
            return StatusCode(500, ApiResponse<Tenant>.ErrorResponse("Failed to create tenant"));
        }
    }

    /// <summary>
    /// Update tenant (Super Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [RequireSuperAdmin]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantDto dto)
    {
        try
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);

            if (tenant == null)
            {
                return NotFound(ApiResponse<Tenant>.ErrorResponse("Tenant not found"));
            }

            tenant.CompanyName = dto.CompanyName ?? tenant.CompanyName;
            tenant.OwnerName = dto.OwnerName ?? tenant.OwnerName;
            tenant.Email = dto.Email ?? tenant.Email;
            tenant.Phone = dto.Phone ?? tenant.Phone;
            tenant.Address = dto.Address;
            tenant.City = dto.City;
            tenant.State = dto.State;
            tenant.PinCode = dto.PinCode;
            tenant.UpdatedAt = DateTime.UtcNow;

            if (dto.MaxMachines.HasValue)
                tenant.MaxMachines = dto.MaxMachines.Value;

            if (dto.MaxWorkers.HasValue)
                tenant.MaxWorkers = dto.MaxWorkers.Value;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated tenant {TenantCode}", tenant.TenantCode);

            return Ok(ApiResponse<Tenant>.SuccessResponse(tenant, "Tenant updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant {TenantId}", id);
            return StatusCode(500, ApiResponse<Tenant>.ErrorResponse("Failed to update tenant"));
        }
    }

    /// <summary>
    /// Activate or deactivate tenant (Super Admin only)
    /// </summary>
    [HttpPatch("{id}/status")]
    [RequireSuperAdmin]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        try
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);

            if (tenant == null)
            {
                return NotFound(ApiResponse<Tenant>.ErrorResponse("Tenant not found"));
            }

            tenant.IsActive = dto.IsActive;
            tenant.UpdatedAt = DateTime.UtcNow;

            if (!dto.IsActive)
            {
                tenant.SubscriptionStatus = "Suspended";
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated tenant {TenantCode} status to {Status}",
                tenant.TenantCode, dto.IsActive ? "Active" : "Inactive");

            return Ok(ApiResponse<Tenant>.SuccessResponse(tenant, "Tenant status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant status {TenantId}", id);
            return StatusCode(500, ApiResponse<Tenant>.ErrorResponse("Failed to update tenant status"));
        }
    }
}

// DTOs
public class CreateTenantDto
{
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PinCode { get; set; }
    public string? SubscriptionPlan { get; set; }
    public int? MaxMachines { get; set; }
    public int? MaxWorkers { get; set; }
    public int? MaxMonthlyBills { get; set; }
}

public class UpdateTenantDto
{
    public string? CompanyName { get; set; }
    public string? OwnerName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public int? MaxMachines { get; set; }
    public int? MaxWorkers { get; set; }
}

public class UpdateStatusDto
{
    public bool IsActive { get; set; }
    public string? Reason { get; set; }
}
