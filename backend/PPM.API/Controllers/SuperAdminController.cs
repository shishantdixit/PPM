using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Admin;
using PPM.Application.DTOs.License;
using PPM.Application.Interfaces;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireSuperAdmin]
public class SuperAdminController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILicenseKeyService _licenseKeyService;
    private readonly ILogger<SuperAdminController> _logger;

    public SuperAdminController(
        ApplicationDbContext dbContext,
        ILicenseKeyService licenseKeyService,
        ILogger<SuperAdminController> logger)
    {
        _dbContext = dbContext;
        _licenseKeyService = licenseKeyService;
        _logger = logger;
    }

    /// <summary>
    /// Get system-wide dashboard statistics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // Tenant counts
            var totalTenants = await _dbContext.Tenants.CountAsync();
            var activeTenants = await _dbContext.Tenants.CountAsync(t => t.SubscriptionStatus == "Active");
            var suspendedTenants = await _dbContext.Tenants.CountAsync(t => t.SubscriptionStatus == "Suspended");
            var expiredTenants = await _dbContext.Tenants.CountAsync(t => t.SubscriptionStatus == "Expired");

            // User count
            var totalUsers = await _dbContext.Users.CountAsync();

            // Sales across all tenants
            var totalSalesAllTime = await _dbContext.FuelSales
                .Where(f => !f.IsVoided)
                .SumAsync(f => f.Amount);
            var totalSalesThisMonth = await _dbContext.FuelSales
                .Where(f => !f.IsVoided && f.CreatedAt >= thisMonth)
                .SumAsync(f => f.Amount);

            // Subscription breakdown
            var basicCount = await _dbContext.Tenants.CountAsync(t => t.SubscriptionPlan == "Basic");
            var premiumCount = await _dbContext.Tenants.CountAsync(t => t.SubscriptionPlan == "Premium");
            var enterpriseCount = await _dbContext.Tenants.CountAsync(t => t.SubscriptionPlan == "Enterprise");

            // Top tenants by sales this month
            var topTenants = await _dbContext.Tenants
                .Select(t => new TenantSummaryDto
                {
                    TenantId = t.TenantId,
                    TenantCode = t.TenantCode,
                    CompanyName = t.CompanyName,
                    OwnerName = t.OwnerName,
                    City = t.City,
                    State = t.State,
                    SubscriptionPlan = t.SubscriptionPlan,
                    SubscriptionStatus = t.SubscriptionStatus,
                    IsActive = t.IsActive,
                    ThisMonthSales = _dbContext.FuelSales
                        .Where(f => f.Shift!.TenantId == t.TenantId && !f.IsVoided && f.CreatedAt >= thisMonth)
                        .Sum(f => f.Amount)
                })
                .OrderByDescending(t => t.ThisMonthSales)
                .Take(5)
                .ToListAsync();

            // Monthly growth (last 6 months)
            var sixMonthsAgo = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-6);
            var monthlyGrowthRaw = await _dbContext.Tenants
                .Where(t => t.CreatedAt >= sixMonthsAgo)
                .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, NewTenants = g.Count() })
                .ToListAsync();

            var monthlyGrowth = monthlyGrowthRaw
                .Select(g => new MonthlyStatsDto
                {
                    Month = $"{g.Year}-{g.Month:D2}",
                    NewTenants = g.NewTenants
                })
                .OrderBy(m => m.Month)
                .ToList();

            // Add sales data to monthly growth
            var monthlySalesRaw = await _dbContext.FuelSales
                .Where(f => !f.IsVoided && f.CreatedAt >= sixMonthsAgo)
                .GroupBy(f => new { f.CreatedAt.Year, f.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Sales = g.Sum(f => f.Amount) })
                .ToListAsync();

            var monthlySales = monthlySalesRaw
                .Select(g => new { Month = $"{g.Year}-{g.Month:D2}", g.Sales })
                .ToList();

            foreach (var month in monthlyGrowth)
            {
                var salesData = monthlySales.FirstOrDefault(s => s.Month == month.Month);
                month.TotalSales = salesData?.Sales ?? 0;
            }

            var dashboard = new SystemDashboardDto
            {
                TotalTenants = totalTenants,
                ActiveTenants = activeTenants,
                SuspendedTenants = suspendedTenants,
                ExpiredTenants = expiredTenants,
                TotalUsers = totalUsers,
                TotalSalesAllTime = totalSalesAllTime,
                TotalSalesThisMonth = totalSalesThisMonth,
                TopTenants = topTenants,
                MonthlyGrowth = monthlyGrowth,
                SubscriptionBreakdown = new SubscriptionBreakdownDto
                {
                    Basic = basicCount,
                    Premium = premiumCount,
                    Enterprise = enterpriseCount
                }
            };

            return Ok(ApiResponse<SystemDashboardDto>.SuccessResponse(dashboard));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching admin dashboard");
            return StatusCode(500, ApiResponse<SystemDashboardDto>.ErrorResponse("Failed to fetch dashboard"));
        }
    }

    /// <summary>
    /// Get system statistics summary
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = new
            {
                TotalTenants = await _dbContext.Tenants.CountAsync(),
                ActiveTenants = await _dbContext.Tenants.CountAsync(t => t.IsActive),
                TotalUsers = await _dbContext.Users.CountAsync(),
                TotalMachines = await _dbContext.Machines.CountAsync(),
                TotalNozzles = await _dbContext.Nozzles.CountAsync(),
                TotalShifts = await _dbContext.Shifts.CountAsync(),
                TotalSales = await _dbContext.FuelSales.Where(f => !f.IsVoided).SumAsync(f => f.Amount),
                TotalTransactions = await _dbContext.FuelSales.CountAsync(f => !f.IsVoided)
            };

            return Ok(ApiResponse<object>.SuccessResponse(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching system stats");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch stats"));
        }
    }

    /// <summary>
    /// Get tenants expiring soon
    /// </summary>
    [HttpGet("expiring-tenants")]
    public async Task<IActionResult> GetExpiringTenants([FromQuery] int days = 30)
    {
        try
        {
            var expirationDate = DateTime.UtcNow.AddDays(days);

            var expiringTenants = await _dbContext.Tenants
                .Where(t => t.SubscriptionEndDate != null &&
                           t.SubscriptionEndDate <= expirationDate &&
                           t.SubscriptionStatus == "Active")
                .OrderBy(t => t.SubscriptionEndDate)
                .Select(t => new
                {
                    t.TenantId,
                    t.TenantCode,
                    t.CompanyName,
                    t.OwnerName,
                    t.Email,
                    t.Phone,
                    t.SubscriptionPlan,
                    t.SubscriptionEndDate,
                    DaysRemaining = (t.SubscriptionEndDate!.Value - DateTime.UtcNow).Days
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.SuccessResponse(expiringTenants));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching expiring tenants");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch expiring tenants"));
        }
    }

    /// <summary>
    /// Get recent activity across system
    /// </summary>
    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int limit = 20)
    {
        try
        {
            var recentTenants = await _dbContext.Tenants
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new { Type = "NewTenant", t.CompanyName, t.TenantCode, Date = t.CreatedAt })
                .ToListAsync();

            var recentLogins = await _dbContext.Users
                .Where(u => u.LastLoginAt != null)
                .OrderByDescending(u => u.LastLoginAt)
                .Take(10)
                .Select(u => new
                {
                    Type = "UserLogin",
                    u.FullName,
                    u.Username,
                    TenantCode = _dbContext.Tenants
                        .Where(t => t.TenantId == u.TenantId)
                        .Select(t => t.TenantCode)
                        .FirstOrDefault(),
                    Date = u.LastLoginAt
                })
                .ToListAsync();

            var activity = new
            {
                RecentTenants = recentTenants,
                RecentLogins = recentLogins
            };

            return Ok(ApiResponse<object>.SuccessResponse(activity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent activity");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch recent activity"));
        }
    }

    #region License Key Management

    /// <summary>
    /// Generate a new license key
    /// </summary>
    [HttpPost("license-keys")]
    public async Task<IActionResult> GenerateLicenseKey([FromBody] GenerateLicenseKeyDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse($"Validation failed: {string.Join(", ", errors)}"));
        }

        try
        {
            var userId = HttpContext.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("User context not found"));
            }

            var licenseKey = await _licenseKeyService.CreateLicenseKeyAsync(dto, userId);

            var response = new LicenseKeyDto
            {
                LicenseKeyId = licenseKey.LicenseKeyId,
                Key = licenseKey.Key,
                SubscriptionPlan = licenseKey.SubscriptionPlan,
                DurationMonths = licenseKey.DurationMonths,
                MaxMachines = licenseKey.MaxMachines,
                MaxWorkers = licenseKey.MaxWorkers,
                MaxMonthlyBills = licenseKey.MaxMonthlyBills,
                Status = licenseKey.Status,
                Notes = licenseKey.Notes,
                CreatedAt = licenseKey.CreatedAt
            };

            _logger.LogInformation("License key generated: {Key} for plan {Plan} by user {UserId}",
                licenseKey.Key, dto.SubscriptionPlan, userId);

            return Ok(ApiResponse<LicenseKeyDto>.SuccessResponse(response, "License key generated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating license key");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to generate license key"));
        }
    }

    /// <summary>
    /// Generate multiple license keys at once
    /// </summary>
    [HttpPost("license-keys/batch")]
    public async Task<IActionResult> GenerateBatchLicenseKeys([FromBody] BatchGenerateLicenseKeyDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse($"Validation failed: {string.Join(", ", errors)}"));
        }

        try
        {
            var userId = HttpContext.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("User context not found"));
            }

            var licenseKeys = await _licenseKeyService.CreateBatchLicenseKeysAsync(dto, userId);

            var response = licenseKeys.Select(lk => new LicenseKeyDto
            {
                LicenseKeyId = lk.LicenseKeyId,
                Key = lk.Key,
                SubscriptionPlan = lk.SubscriptionPlan,
                DurationMonths = lk.DurationMonths,
                MaxMachines = lk.MaxMachines,
                MaxWorkers = lk.MaxWorkers,
                MaxMonthlyBills = lk.MaxMonthlyBills,
                Status = lk.Status,
                Notes = lk.Notes,
                CreatedAt = lk.CreatedAt
            }).ToList();

            _logger.LogInformation("Batch generated {Count} license keys for plan {Plan} by user {UserId}",
                dto.Count, dto.SubscriptionPlan, userId);

            return Ok(ApiResponse<List<LicenseKeyDto>>.SuccessResponse(response,
                $"{dto.Count} license keys generated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating batch license keys");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to generate license keys"));
        }
    }

    /// <summary>
    /// Get all license keys with filtering
    /// </summary>
    [HttpGet("license-keys")]
    public async Task<IActionResult> GetLicenseKeys([FromQuery] LicenseKeyQueryDto query)
    {
        try
        {
            var (keys, totalCount) = await _licenseKeyService.GetLicenseKeysAsync(query);

            var response = new
            {
                Keys = keys,
                TotalCount = totalCount,
                Page = query.Page,
                Limit = query.Limit,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.Limit)
            };

            return Ok(ApiResponse<object>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching license keys");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch license keys"));
        }
    }

    /// <summary>
    /// Get a specific license key by ID
    /// </summary>
    [HttpGet("license-keys/{id}")]
    public async Task<IActionResult> GetLicenseKey(Guid id)
    {
        try
        {
            var licenseKey = await _licenseKeyService.GetByIdAsync(id);

            if (licenseKey == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("License key not found"));
            }

            var response = new LicenseKeyDto
            {
                LicenseKeyId = licenseKey.LicenseKeyId,
                Key = licenseKey.Key,
                SubscriptionPlan = licenseKey.SubscriptionPlan,
                DurationMonths = licenseKey.DurationMonths,
                MaxMachines = licenseKey.MaxMachines,
                MaxWorkers = licenseKey.MaxWorkers,
                MaxMonthlyBills = licenseKey.MaxMonthlyBills,
                Status = licenseKey.Status,
                ActivatedByTenantId = licenseKey.ActivatedByTenantId,
                ActivatedByTenantCode = licenseKey.ActivatedByTenant?.TenantCode,
                ActivatedByCompanyName = licenseKey.ActivatedByTenant?.CompanyName,
                ActivatedAt = licenseKey.ActivatedAt,
                GeneratedByUsername = licenseKey.GeneratedBySystemUser.Username,
                Notes = licenseKey.Notes,
                CreatedAt = licenseKey.CreatedAt
            };

            return Ok(ApiResponse<LicenseKeyDto>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching license key {LicenseKeyId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch license key"));
        }
    }

    /// <summary>
    /// Revoke an unused license key
    /// </summary>
    [HttpPatch("license-keys/{id}/revoke")]
    public async Task<IActionResult> RevokeLicenseKey(Guid id, [FromBody] RevokeLicenseKeyDto dto)
    {
        try
        {
            var result = await _licenseKeyService.RevokeKeyAsync(id, dto.Reason);

            if (!result)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Cannot revoke this license key. It may not exist or has already been used."));
            }

            _logger.LogInformation("License key {LicenseKeyId} revoked. Reason: {Reason}",
                id, dto.Reason ?? "Not specified");

            return Ok(ApiResponse<object>.SuccessResponse(null, "License key revoked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking license key {LicenseKeyId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to revoke license key"));
        }
    }

    /// <summary>
    /// Get license key statistics
    /// </summary>
    [HttpGet("license-keys/stats")]
    public async Task<IActionResult> GetLicenseKeyStats()
    {
        try
        {
            var stats = new
            {
                TotalKeys = await _dbContext.LicenseKeys.CountAsync(),
                AvailableKeys = await _dbContext.LicenseKeys.CountAsync(lk => lk.Status == "Available"),
                UsedKeys = await _dbContext.LicenseKeys.CountAsync(lk => lk.Status == "Used"),
                RevokedKeys = await _dbContext.LicenseKeys.CountAsync(lk => lk.Status == "Revoked"),
                KeysByPlan = await _dbContext.LicenseKeys
                    .Where(lk => lk.Status == "Available")
                    .GroupBy(lk => lk.SubscriptionPlan)
                    .Select(g => new { Plan = g.Key, Count = g.Count() })
                    .ToListAsync()
            };

            return Ok(ApiResponse<object>.SuccessResponse(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching license key stats");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch license key statistics"));
        }
    }

    #endregion
}
