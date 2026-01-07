using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.Application.Common;
using PPM.Application.DTOs.Admin;
using PPM.Application.Interfaces;
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
    private readonly IPasswordHasher _passwordHasher;

    public TenantsController(ApplicationDbContext dbContext, ILogger<TenantsController> logger, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _logger = logger;
        _passwordHasher = passwordHasher;
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
            else
            {
                tenant.SubscriptionStatus = "Active";
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

    /// <summary>
    /// Get tenant with detailed stats (Super Admin only)
    /// </summary>
    [HttpGet("{id}/details")]
    [RequireSuperAdmin]
    public async Task<IActionResult> GetTenantDetails(Guid id)
    {
        try
        {
            var tenant = await _dbContext.Tenants
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.TenantId == id);

            if (tenant == null)
            {
                return NotFound(ApiResponse<TenantDetailDto>.ErrorResponse("Tenant not found"));
            }

            var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // Get stats
            var machineCount = await _dbContext.Machines.CountAsync(m => m.TenantId == id);
            var nozzleCount = await _dbContext.Nozzles.CountAsync(n => n.TenantId == id);
            var totalSales = await _dbContext.FuelSales.Where(f => f.Shift!.TenantId == id && !f.IsVoided).SumAsync(f => f.Amount);
            var thisMonthSales = await _dbContext.FuelSales
                .Where(f => f.Shift!.TenantId == id && !f.IsVoided && f.CreatedAt >= thisMonth)
                .SumAsync(f => f.Amount);
            var totalShifts = await _dbContext.Shifts.CountAsync(s => s.TenantId == id);

            // Get last 6 months sales
            var sixMonthsAgo = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-6);
            var monthlySalesRaw = await _dbContext.FuelSales
                .Where(f => f.Shift!.TenantId == id && !f.IsVoided && f.CreatedAt >= sixMonthsAgo)
                .GroupBy(f => new { f.CreatedAt.Year, f.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Sales = g.Sum(f => f.Amount),
                    ShiftCount = g.Select(f => f.ShiftId).Distinct().Count()
                })
                .ToListAsync();

            var monthlySales = monthlySalesRaw
                .Select(g => new MonthlySalesDto
                {
                    Month = $"{g.Year}-{g.Month:D2}",
                    Sales = g.Sales,
                    ShiftCount = g.ShiftCount
                })
                .OrderBy(m => m.Month)
                .ToList();

            var detail = new TenantDetailDto
            {
                TenantId = tenant.TenantId,
                TenantCode = tenant.TenantCode,
                CompanyName = tenant.CompanyName,
                OwnerName = tenant.OwnerName,
                Email = tenant.Email,
                Phone = tenant.Phone,
                Address = tenant.Address,
                City = tenant.City,
                State = tenant.State,
                Country = tenant.Country,
                PinCode = tenant.PinCode,
                SubscriptionPlan = tenant.SubscriptionPlan,
                SubscriptionStatus = tenant.SubscriptionStatus,
                SubscriptionStartDate = tenant.SubscriptionStartDate,
                SubscriptionEndDate = tenant.SubscriptionEndDate,
                MaxMachines = tenant.MaxMachines,
                MaxWorkers = tenant.MaxWorkers,
                MaxMonthlyBills = tenant.MaxMonthlyBills,
                IsActive = tenant.IsActive,
                CreatedAt = tenant.CreatedAt,
                UpdatedAt = tenant.UpdatedAt,
                UserCount = tenant.Users.Count,
                MachineCount = machineCount,
                NozzleCount = nozzleCount,
                TotalSales = totalSales,
                ThisMonthSales = thisMonthSales,
                TotalShifts = totalShifts,
                Users = tenant.Users.Select(u => new TenantUserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt
                }).ToList(),
                SalesHistory = monthlySales
            };

            return Ok(ApiResponse<TenantDetailDto>.SuccessResponse(detail));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenant details {TenantId}", id);
            return StatusCode(500, ApiResponse<TenantDetailDto>.ErrorResponse("Failed to fetch tenant details"));
        }
    }

    /// <summary>
    /// Update tenant subscription (Super Admin only)
    /// </summary>
    [HttpPatch("{id}/subscription")]
    [RequireSuperAdmin]
    public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] UpdateSubscriptionDto dto)
    {
        try
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);

            if (tenant == null)
            {
                return NotFound(ApiResponse<Tenant>.ErrorResponse("Tenant not found"));
            }

            if (!string.IsNullOrEmpty(dto.SubscriptionPlan))
                tenant.SubscriptionPlan = dto.SubscriptionPlan;

            if (dto.SubscriptionEndDate.HasValue)
                tenant.SubscriptionEndDate = dto.SubscriptionEndDate.Value;

            if (dto.MaxMachines.HasValue)
                tenant.MaxMachines = dto.MaxMachines.Value;

            if (dto.MaxWorkers.HasValue)
                tenant.MaxWorkers = dto.MaxWorkers.Value;

            if (dto.MaxMonthlyBills.HasValue)
                tenant.MaxMonthlyBills = dto.MaxMonthlyBills.Value;

            tenant.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated subscription for tenant {TenantCode}", tenant.TenantCode);

            return Ok(ApiResponse<Tenant>.SuccessResponse(tenant, "Subscription updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subscription {TenantId}", id);
            return StatusCode(500, ApiResponse<Tenant>.ErrorResponse("Failed to update subscription"));
        }
    }

    /// <summary>
    /// Create owner user for a tenant (Super Admin only)
    /// </summary>
    [HttpPost("{id}/owner")]
    [RequireSuperAdmin]
    public async Task<IActionResult> CreateOwnerUser(Guid id, [FromBody] CreateOwnerUserDto dto)
    {
        try
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);

            if (tenant == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Tenant not found"));
            }

            // Check if username already exists in this tenant
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.TenantId == id && u.Username == dto.Username);

            if (existingUser != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Username already exists"));
            }

            // Check if there's already an owner
            var existingOwner = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.TenantId == id && u.Role == "Owner");

            if (existingOwner != null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant already has an owner. Please edit the existing owner instead."));
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                TenantId = id,
                Username = dto.Username,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Owner",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created owner user for tenant {TenantCode}", tenant.TenantCode);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                user.UserId,
                user.Username,
                user.FullName,
                user.Role
            }, "Owner user created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating owner for tenant {TenantId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to create owner user"));
        }
    }

    /// <summary>
    /// Get tenants with summary stats (Super Admin only)
    /// </summary>
    [HttpGet("with-stats")]
    [RequireSuperAdmin]
    public async Task<IActionResult> GetAllWithStats(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? plan = null,
        [FromQuery] string? search = null)
    {
        try
        {
            var query = _dbContext.Tenants.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.SubscriptionStatus == status);

            if (!string.IsNullOrEmpty(plan))
                query = query.Where(t => t.SubscriptionPlan == plan);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(t =>
                    t.CompanyName.ToLower().Contains(search) ||
                    t.OwnerName.ToLower().Contains(search) ||
                    t.TenantCode.ToLower().Contains(search) ||
                    t.City!.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var tenants = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var tenantSummaries = new List<TenantSummaryDto>();

            foreach (var tenant in tenants)
            {
                var userCount = await _dbContext.Users.CountAsync(u => u.TenantId == tenant.TenantId);
                var machineCount = await _dbContext.Machines.CountAsync(m => m.TenantId == tenant.TenantId);
                var nozzleCount = await _dbContext.Nozzles.CountAsync(n => n.TenantId == tenant.TenantId);
                var totalSales = await _dbContext.FuelSales
                    .Where(f => f.Shift!.TenantId == tenant.TenantId && !f.IsVoided)
                    .SumAsync(f => f.Amount);
                var thisMonthSales = await _dbContext.FuelSales
                    .Where(f => f.Shift!.TenantId == tenant.TenantId && !f.IsVoided && f.CreatedAt >= thisMonth)
                    .SumAsync(f => f.Amount);
                var totalShifts = await _dbContext.Shifts.CountAsync(s => s.TenantId == tenant.TenantId);

                tenantSummaries.Add(new TenantSummaryDto
                {
                    TenantId = tenant.TenantId,
                    TenantCode = tenant.TenantCode,
                    CompanyName = tenant.CompanyName,
                    OwnerName = tenant.OwnerName,
                    Email = tenant.Email,
                    Phone = tenant.Phone,
                    City = tenant.City,
                    State = tenant.State,
                    SubscriptionPlan = tenant.SubscriptionPlan,
                    SubscriptionStatus = tenant.SubscriptionStatus,
                    SubscriptionStartDate = tenant.SubscriptionStartDate,
                    SubscriptionEndDate = tenant.SubscriptionEndDate,
                    MaxMachines = tenant.MaxMachines,
                    MaxWorkers = tenant.MaxWorkers,
                    IsActive = tenant.IsActive,
                    CreatedAt = tenant.CreatedAt,
                    UserCount = userCount,
                    MachineCount = machineCount,
                    NozzleCount = nozzleCount,
                    TotalSales = totalSales,
                    ThisMonthSales = thisMonthSales,
                    TotalShifts = totalShifts
                });
            }

            var response = new PagedResponse<TenantSummaryDto>
            {
                Items = tenantSummaries,
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };

            return Ok(ApiResponse<PagedResponse<TenantSummaryDto>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenants with stats");
            return StatusCode(500, ApiResponse<PagedResponse<TenantSummaryDto>>.ErrorResponse("Failed to fetch tenants"));
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
