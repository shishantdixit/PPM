using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.Application.Common;
using PPM.Application.DTOs.Admin;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireSuperAdmin]
public class SuperAdminController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SuperAdminController> _logger;

    public SuperAdminController(ApplicationDbContext dbContext, ILogger<SuperAdminController> logger)
    {
        _dbContext = dbContext;
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
}
