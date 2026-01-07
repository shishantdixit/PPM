using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.API.Services;
using PPM.Application.Common;
using PPM.Application.DTOs.Reports;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ReportsController> _logger;
    private readonly IExportService _exportService;

    public ReportsController(ApplicationDbContext dbContext, ILogger<ReportsController> logger, IExportService exportService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _exportService = exportService;
    }

    /// <summary>
    /// Get dashboard summary with key metrics
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<DashboardSummaryDto>.ErrorResponse("Tenant context not found"));

            // Use local time for "today" to match user's expectation
            // Sales SaleTime is stored in UTC, but we compare dates which should work correctly
            var now = DateTime.Now;  // Local time
            var today = DateOnly.FromDateTime(now);
            var yesterday = today.AddDays(-1);
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var lastWeekStart = weekStart.AddDays(-7);
            var monthStart = new DateOnly(today.Year, today.Month, 1);
            var lastMonthStart = monthStart.AddMonths(-1);

            // Today's sales - use shift data (TotalSales from meter readings)
            // This is more accurate as it reflects actual fuel dispensed
            var todayShiftSales = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.ShiftDate == today)
                .Include(s => s.NozzleReadings)
                .ToListAsync();

            var todaySalesAmount = todayShiftSales.Sum(s => s.TotalSales);
            var todayQuantity = todayShiftSales
                .SelectMany(s => s.NozzleReadings)
                .Sum(nr => nr.QuantitySold);
            var todaySalesCount = todayShiftSales.Count;

            // Yesterday's sales for comparison
            var yesterdayShiftSales = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.ShiftDate == yesterday)
                .SumAsync(s => s.TotalSales);

            // This week's sales - use shift data
            var weekShiftSales = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.ShiftDate >= weekStart)
                .Include(s => s.NozzleReadings)
                .ToListAsync();

            var weekSalesAmount = weekShiftSales.Sum(s => s.TotalSales);
            var weekQuantity = weekShiftSales
                .SelectMany(s => s.NozzleReadings)
                .Sum(nr => nr.QuantitySold);

            // Last week's sales for comparison
            var lastWeekShiftSales = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.ShiftDate >= lastWeekStart && s.ShiftDate < weekStart)
                .SumAsync(s => s.TotalSales);

            // This month's sales - use shift data
            var monthShiftSales = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.ShiftDate >= monthStart)
                .Include(s => s.NozzleReadings)
                .ToListAsync();

            var monthSalesAmount = monthShiftSales.Sum(s => s.TotalSales);
            var monthQuantity = monthShiftSales
                .SelectMany(s => s.NozzleReadings)
                .Sum(nr => nr.QuantitySold);

            // Last month's sales for comparison
            var lastMonthShiftSales = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.ShiftDate >= lastMonthStart && s.ShiftDate < monthStart)
                .SumAsync(s => s.TotalSales);

            // Active shifts count
            var activeShifts = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.Status == ShiftStatus.Active)
                .CountAsync();

            // Top fuel types (this month) - use shift nozzle readings data
            var monthNozzleReadings = await _dbContext.ShiftNozzleReadings
                .Where(nr => nr.Shift!.TenantId == tenantId.Value && nr.Shift.ShiftDate >= monthStart)
                .Include(nr => nr.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .ToListAsync();

            var topFuelTypes = monthNozzleReadings
                .GroupBy(nr => new { nr.Nozzle!.FuelTypeId, nr.Nozzle.FuelType!.FuelName, nr.Nozzle.FuelType.FuelCode })
                .Select(g => new FuelTypePerformanceDto
                {
                    FuelTypeId = g.Key.FuelTypeId,
                    FuelName = g.Key.FuelName,
                    FuelCode = g.Key.FuelCode,
                    TotalQuantity = g.Sum(nr => nr.QuantitySold),
                    TotalAmount = g.Sum(nr => nr.ExpectedAmount),
                    SalesCount = g.Count()
                })
                .OrderByDescending(f => f.TotalAmount)
                .Take(5)
                .ToList();

            // Last 7 days trend - use shift data
            var sevenDaysAgo = today.AddDays(-6);
            var trendShifts = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value && s.ShiftDate >= sevenDaysAgo)
                .Include(s => s.NozzleReadings)
                .ToListAsync();

            // Group by shift date
            var trendGrouped = trendShifts
                .GroupBy(s => s.ShiftDate)
                .Select(g => new SalesTrendDto
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(s => s.TotalSales),
                    TotalQuantity = g.SelectMany(s => s.NozzleReadings).Sum(nr => nr.QuantitySold),
                    SalesCount = g.Count()  // Number of shifts
                })
                .OrderBy(t => t.Date)
                .ToList();

            // Fill in missing days with zero values
            var last7DaysTrend = new List<SalesTrendDto>();
            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var existing = trendGrouped.FirstOrDefault(t => t.Date == date);
                last7DaysTrend.Add(existing ?? new SalesTrendDto { Date = date, TotalAmount = 0, TotalQuantity = 0, SalesCount = 0 });
            }

            var summary = new DashboardSummaryDto
            {
                Date = today,
                TodaySales = todaySalesAmount,
                TodayQuantity = todayQuantity,
                TodaySalesCount = todaySalesCount,
                ActiveShifts = activeShifts,
                WeekSales = weekSalesAmount,
                WeekQuantity = weekQuantity,
                MonthSales = monthSalesAmount,
                MonthQuantity = monthQuantity,
                SalesChangePercent = yesterdayShiftSales > 0 ? (todaySalesAmount - yesterdayShiftSales) / yesterdayShiftSales * 100 : 0,
                WeekChangePercent = lastWeekShiftSales > 0 ? (weekSalesAmount - lastWeekShiftSales) / lastWeekShiftSales * 100 : 0,
                MonthChangePercent = lastMonthShiftSales > 0 ? (monthSalesAmount - lastMonthShiftSales) / lastMonthShiftSales * 100 : 0,
                TopFuelTypes = topFuelTypes,
                Last7DaysTrend = last7DaysTrend
            };

            return Ok(ApiResponse<DashboardSummaryDto>.SuccessResponse(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating dashboard summary");
            return StatusCode(500, ApiResponse<DashboardSummaryDto>.ErrorResponse("Failed to generate dashboard summary"));
        }
    }

    /// <summary>
    /// Get daily sales summary for a date range
    /// </summary>
    [HttpGet("daily-sales")]
    [RequireManager]
    public async Task<IActionResult> GetDailySalesSummary(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<DailySalesSummaryDto>>.ErrorResponse("Tenant context not found"));

            var today = DateOnly.FromDateTime(DateTime.Now);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            // Use shift data for sales (meter readings based)
            var shifts = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value)
                .Where(s => s.ShiftDate >= from && s.ShiftDate <= to)
                .ToListAsync();

            // Group shifts by date
            var dailySales = shifts
                .GroupBy(s => s.ShiftDate)
                .Select(g => new DailySalesSummaryDto
                {
                    Date = g.Key,
                    TotalSales = g.Count(), // Number of shifts
                    VoidedSales = 0, // Not applicable for shift-based tracking
                    TotalQuantity = 0, // Will be calculated from nozzle readings
                    TotalAmount = g.Sum(s => s.TotalSales),
                    CashAmount = g.Sum(s => s.CashCollected),
                    CreditAmount = g.Sum(s => s.CreditSales),
                    DigitalAmount = g.Sum(s => s.DigitalPayments),
                    TotalShifts = g.Count()
                })
                .OrderByDescending(d => d.Date)
                .ToList();

            // Get quantity from nozzle readings for each day
            var nozzleReadings = await _dbContext.ShiftNozzleReadings
                .Where(nr => nr.Shift!.TenantId == tenantId.Value && nr.Shift.ShiftDate >= from && nr.Shift.ShiftDate <= to)
                .Include(nr => nr.Shift)
                .ToListAsync();

            var quantityByDate = nozzleReadings
                .GroupBy(nr => nr.Shift!.ShiftDate)
                .ToDictionary(g => g.Key, g => g.Sum(nr => nr.QuantitySold));

            foreach (var day in dailySales)
            {
                if (quantityByDate.TryGetValue(day.Date, out var quantity))
                {
                    day.TotalQuantity = quantity;
                }
            }

            return Ok(ApiResponse<List<DailySalesSummaryDto>>.SuccessResponse(dailySales));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating daily sales summary");
            return StatusCode(500, ApiResponse<List<DailySalesSummaryDto>>.ErrorResponse("Failed to generate daily sales summary"));
        }
    }

    /// <summary>
    /// Get shift summary report
    /// </summary>
    [HttpGet("shifts")]
    [RequireManager]
    public async Task<IActionResult> GetShiftSummaryReport(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? workerId)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<ShiftSummaryReportDto>>.ErrorResponse("Tenant context not found"));

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            var query = _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value)
                .Where(s => s.ShiftDate >= from && s.ShiftDate <= to)
                .Include(s => s.Worker)
                .Include(s => s.FuelSales)
                .AsQueryable();

            if (workerId.HasValue)
                query = query.Where(s => s.WorkerId == workerId.Value);

            var shifts = await query
                .Select(s => new ShiftSummaryReportDto
                {
                    ShiftId = s.ShiftId,
                    ShiftDate = s.ShiftDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    WorkerName = s.Worker!.FullName,
                    Status = s.Status,
                    SalesCount = s.FuelSales.Count(fs => !fs.IsVoided),
                    TotalQuantity = s.FuelSales.Where(fs => !fs.IsVoided).Sum(fs => fs.Quantity),
                    TotalSales = s.TotalSales,
                    CashCollected = s.CashCollected,
                    CreditSales = s.CreditSales,
                    DigitalPayments = s.DigitalPayments,
                    Variance = s.Variance
                })
                .OrderByDescending(s => s.ShiftDate)
                .ThenByDescending(s => s.StartTime)
                .ToListAsync();

            return Ok(ApiResponse<List<ShiftSummaryReportDto>>.SuccessResponse(shifts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating shift summary report");
            return StatusCode(500, ApiResponse<List<ShiftSummaryReportDto>>.ErrorResponse("Failed to generate shift summary report"));
        }
    }

    /// <summary>
    /// Get fuel type performance report
    /// </summary>
    [HttpGet("fuel-performance")]
    [RequireManager]
    public async Task<IActionResult> GetFuelTypePerformance(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<FuelTypePerformanceDto>>.ErrorResponse("Tenant context not found"));

            var today = DateOnly.FromDateTime(DateTime.Now);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            // Use shift nozzle readings instead of FuelSales
            var readings = await _dbContext.ShiftNozzleReadings
                .Where(nr => nr.Shift!.TenantId == tenantId.Value)
                .Where(nr => nr.Shift!.ShiftDate >= from && nr.Shift!.ShiftDate <= to)
                .Include(nr => nr.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .ToListAsync();

            var performance = readings
                .GroupBy(nr => new { nr.Nozzle!.FuelTypeId, nr.Nozzle.FuelType!.FuelName, nr.Nozzle.FuelType.FuelCode })
                .Select(g => new FuelTypePerformanceDto
                {
                    FuelTypeId = g.Key.FuelTypeId,
                    FuelName = g.Key.FuelName,
                    FuelCode = g.Key.FuelCode,
                    TotalQuantity = g.Sum(nr => nr.QuantitySold),
                    TotalAmount = g.Sum(nr => nr.ExpectedAmount),
                    AverageRate = g.Average(nr => nr.RateAtShift),
                    SalesCount = g.Count()
                })
                .OrderByDescending(f => f.TotalAmount)
                .ToList();

            // Calculate percentage of total
            var totalAmount = performance.Sum(p => p.TotalAmount);
            foreach (var p in performance)
            {
                p.PercentageOfTotal = totalAmount > 0 ? p.TotalAmount / totalAmount * 100 : 0;
            }

            return Ok(ApiResponse<List<FuelTypePerformanceDto>>.SuccessResponse(performance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating fuel type performance report");
            return StatusCode(500, ApiResponse<List<FuelTypePerformanceDto>>.ErrorResponse("Failed to generate fuel type performance report"));
        }
    }

    /// <summary>
    /// Get worker performance report
    /// </summary>
    [HttpGet("worker-performance")]
    [RequireOwner]
    public async Task<IActionResult> GetWorkerPerformance(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<WorkerPerformanceDto>>.ErrorResponse("Tenant context not found"));

            var today = DateOnly.FromDateTime(DateTime.Now);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            // Get shifts with nozzle readings for quantity calculation
            var shifts = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value)
                .Where(s => s.ShiftDate >= from && s.ShiftDate <= to)
                .Include(s => s.Worker)
                .Include(s => s.NozzleReadings)
                .ToListAsync();

            // Group by worker and calculate performance
            var performance = shifts
                .GroupBy(s => new { s.WorkerId, WorkerName = s.Worker?.FullName ?? "Unknown" })
                .Select(g => new WorkerPerformanceDto
                {
                    UserId = g.Key.WorkerId,
                    WorkerName = g.Key.WorkerName,
                    TotalShifts = g.Count(),
                    TotalSales = g.Count(), // Number of shifts worked
                    TotalAmount = g.Sum(s => s.TotalSales),
                    TotalQuantity = g.SelectMany(s => s.NozzleReadings).Sum(nr => nr.QuantitySold),
                    TotalVariance = g.Sum(s => s.Variance)
                })
                .OrderByDescending(w => w.TotalAmount)
                .ToList();

            // Calculate average shift sales
            foreach (var w in performance)
            {
                w.AverageShiftSales = w.TotalShifts > 0 ? w.TotalAmount / w.TotalShifts : 0;
            }

            return Ok(ApiResponse<List<WorkerPerformanceDto>>.SuccessResponse(performance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating worker performance report");
            return StatusCode(500, ApiResponse<List<WorkerPerformanceDto>>.ErrorResponse("Failed to generate worker performance report"));
        }
    }

    /// <summary>
    /// Get nozzle performance report
    /// </summary>
    [HttpGet("nozzle-performance")]
    [RequireManager]
    public async Task<IActionResult> GetNozzlePerformance(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? machineId)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<NozzlePerformanceDto>>.ErrorResponse("Tenant context not found"));

            var today = DateOnly.FromDateTime(DateTime.Now);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            // Use shift nozzle readings instead of FuelSales
            var query = _dbContext.ShiftNozzleReadings
                .Where(nr => nr.Shift!.TenantId == tenantId.Value)
                .Where(nr => nr.Shift!.ShiftDate >= from && nr.Shift!.ShiftDate <= to)
                .Include(nr => nr.Nozzle)
                    .ThenInclude(n => n!.Machine)
                .Include(nr => nr.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .AsQueryable();

            if (machineId.HasValue)
                query = query.Where(nr => nr.Nozzle!.MachineId == machineId.Value);

            var readings = await query.ToListAsync();

            var performance = readings
                .GroupBy(nr => new
                {
                    nr.NozzleId,
                    nr.Nozzle!.NozzleNumber,
                    MachineName = nr.Nozzle.Machine!.MachineName,
                    FuelType = nr.Nozzle.FuelType!.FuelName
                })
                .Select(g => new NozzlePerformanceDto
                {
                    NozzleId = g.Key.NozzleId,
                    NozzleNumber = g.Key.NozzleNumber,
                    MachineName = g.Key.MachineName,
                    FuelType = g.Key.FuelType,
                    TotalQuantity = g.Sum(nr => nr.QuantitySold),
                    TotalAmount = g.Sum(nr => nr.ExpectedAmount),
                    SalesCount = g.Count() // Number of shift readings
                })
                .OrderByDescending(n => n.TotalAmount)
                .ToList();

            return Ok(ApiResponse<List<NozzlePerformanceDto>>.SuccessResponse(performance));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating nozzle performance report");
            return StatusCode(500, ApiResponse<List<NozzlePerformanceDto>>.ErrorResponse("Failed to generate nozzle performance report"));
        }
    }

    /// <summary>
    /// Get sales trend data for charts
    /// </summary>
    [HttpGet("sales-trend")]
    [RequireManager]
    public async Task<IActionResult> GetSalesTrend(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<SalesTrendDto>>.ErrorResponse("Tenant context not found"));

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            var trendData = await _dbContext.FuelSales
                .Where(fs => fs.TenantId == tenantId.Value && !fs.IsVoided)
                .Where(fs => DateOnly.FromDateTime(fs.SaleTime) >= from && DateOnly.FromDateTime(fs.SaleTime) <= to)
                .GroupBy(fs => DateOnly.FromDateTime(fs.SaleTime))
                .Select(g => new SalesTrendDto
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(fs => fs.Amount),
                    TotalQuantity = g.Sum(fs => fs.Quantity),
                    SalesCount = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            // Fill in missing days
            var allDays = new List<SalesTrendDto>();
            for (var date = from; date <= to; date = date.AddDays(1))
            {
                var existing = trendData.FirstOrDefault(t => t.Date == date);
                allDays.Add(existing ?? new SalesTrendDto { Date = date, TotalAmount = 0, TotalQuantity = 0, SalesCount = 0 });
            }

            return Ok(ApiResponse<List<SalesTrendDto>>.SuccessResponse(allDays));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sales trend data");
            return StatusCode(500, ApiResponse<List<SalesTrendDto>>.ErrorResponse("Failed to generate sales trend data"));
        }
    }

    /// <summary>
    /// Export daily sales report
    /// </summary>
    [HttpGet("daily-sales/export")]
    [RequireManager]
    public async Task<IActionResult> ExportDailySales(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            var dailySales = await _dbContext.FuelSales
                .Where(fs => fs.TenantId == tenantId.Value)
                .Where(fs => DateOnly.FromDateTime(fs.SaleTime) >= from && DateOnly.FromDateTime(fs.SaleTime) <= to)
                .GroupBy(fs => DateOnly.FromDateTime(fs.SaleTime))
                .Select(g => new
                {
                    Date = g.Key,
                    TotalSales = g.Count(fs => !fs.IsVoided),
                    VoidedSales = g.Count(fs => fs.IsVoided),
                    TotalQuantity = g.Where(fs => !fs.IsVoided).Sum(fs => fs.Quantity),
                    TotalAmount = g.Where(fs => !fs.IsVoided).Sum(fs => fs.Amount),
                    CashAmount = g.Where(fs => !fs.IsVoided && fs.PaymentMethod == PaymentMethod.Cash).Sum(fs => fs.Amount),
                    CreditAmount = g.Where(fs => !fs.IsVoided && fs.PaymentMethod == PaymentMethod.Credit).Sum(fs => fs.Amount),
                    DigitalAmount = g.Where(fs => !fs.IsVoided && fs.PaymentMethod == PaymentMethod.Digital).Sum(fs => fs.Amount)
                })
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            var headers = new[] { "Date", "Sales", "Voided", "Quantity (L)", "Total Amount", "Cash", "Credit", "Digital" };
            var rows = dailySales.Select(d => new[]
            {
                d.Date.ToString("dd-MMM-yyyy"),
                d.TotalSales.ToString(),
                d.VoidedSales.ToString(),
                d.TotalQuantity.ToString("N2"),
                d.TotalAmount.ToString("N2"),
                d.CashAmount.ToString("N2"),
                d.CreditAmount.ToString("N2"),
                d.DigitalAmount.ToString("N2")
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Sales", dailySales.Sum(d => d.TotalSales).ToString() },
                { "Total Amount", dailySales.Sum(d => d.TotalAmount).ToString("N2") },
                { "Total Quantity", dailySales.Sum(d => d.TotalQuantity).ToString("N2") + " L" }
            };

            return GenerateExport($"Daily Sales Report ({from:dd-MMM} to {to:dd-MMM})", headers, rows, summary, format, "daily-sales");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting daily sales report");
            return StatusCode(500, "Failed to export report");
        }
    }

    /// <summary>
    /// Export shift summary report
    /// </summary>
    [HttpGet("shifts/export")]
    [RequireManager]
    public async Task<IActionResult> ExportShiftSummary(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? workerId,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            var query = _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value)
                .Where(s => s.ShiftDate >= from && s.ShiftDate <= to)
                .Include(s => s.Worker)
                .Include(s => s.FuelSales)
                .AsQueryable();

            if (workerId.HasValue)
                query = query.Where(s => s.WorkerId == workerId.Value);

            var shifts = await query
                .OrderByDescending(s => s.ShiftDate)
                .ThenByDescending(s => s.StartTime)
                .ToListAsync();

            var headers = new[] { "Date", "Time", "Worker", "Status", "Sales", "Quantity (L)", "Total", "Cash", "Credit", "Digital", "Variance" };
            var rows = shifts.Select(s => new[]
            {
                s.ShiftDate.ToString("dd-MMM-yyyy"),
                $"{s.StartTime:hh\\:mm} - {(s.EndTime.HasValue ? s.EndTime.Value.ToString("hh\\:mm") : "Active")}",
                s.Worker?.FullName ?? "Unknown",
                s.Status.ToString(),
                s.FuelSales.Count(fs => !fs.IsVoided).ToString(),
                s.FuelSales.Where(fs => !fs.IsVoided).Sum(fs => fs.Quantity).ToString("N2"),
                s.TotalSales.ToString("N2"),
                s.CashCollected.ToString("N2"),
                s.CreditSales.ToString("N2"),
                s.DigitalPayments.ToString("N2"),
                s.Variance.ToString("N2")
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Shifts", shifts.Count.ToString() },
                { "Total Sales", shifts.Sum(s => s.TotalSales).ToString("N2") },
                { "Total Variance", shifts.Sum(s => s.Variance).ToString("N2") }
            };

            return GenerateExport($"Shift Summary Report ({from:dd-MMM} to {to:dd-MMM})", headers, rows, summary, format, "shift-summary");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting shift summary report");
            return StatusCode(500, "Failed to export report");
        }
    }

    /// <summary>
    /// Export fuel performance report
    /// </summary>
    [HttpGet("fuel-performance/export")]
    [RequireManager]
    public async Task<IActionResult> ExportFuelPerformance(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            var performance = await _dbContext.FuelSales
                .Where(fs => fs.TenantId == tenantId.Value && !fs.IsVoided)
                .Where(fs => DateOnly.FromDateTime(fs.SaleTime) >= from && DateOnly.FromDateTime(fs.SaleTime) <= to)
                .Include(fs => fs.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .GroupBy(fs => new { fs.Nozzle!.FuelTypeId, fs.Nozzle.FuelType!.FuelName, fs.Nozzle.FuelType.FuelCode })
                .Select(g => new
                {
                    FuelName = g.Key.FuelName,
                    FuelCode = g.Key.FuelCode,
                    TotalQuantity = g.Sum(fs => fs.Quantity),
                    TotalAmount = g.Sum(fs => fs.Amount),
                    AverageRate = g.Average(fs => fs.Rate),
                    SalesCount = g.Count()
                })
                .OrderByDescending(f => f.TotalAmount)
                .ToListAsync();

            var totalAmount = performance.Sum(p => p.TotalAmount);

            var headers = new[] { "Fuel Type", "Code", "Quantity (L)", "Amount", "Avg Rate", "Sales Count", "% of Total" };
            var rows = performance.Select(p => new[]
            {
                p.FuelName,
                p.FuelCode,
                p.TotalQuantity.ToString("N2"),
                p.TotalAmount.ToString("N2"),
                p.AverageRate.ToString("N2"),
                p.SalesCount.ToString(),
                (totalAmount > 0 ? (p.TotalAmount / totalAmount * 100) : 0).ToString("N1") + "%"
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Fuel Types", performance.Count.ToString() },
                { "Total Sales", totalAmount.ToString("N2") },
                { "Total Quantity", performance.Sum(p => p.TotalQuantity).ToString("N2") + " L" }
            };

            return GenerateExport($"Fuel Performance Report ({from:dd-MMM} to {to:dd-MMM})", headers, rows, summary, format, "fuel-performance");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting fuel performance report");
            return StatusCode(500, "Failed to export report");
        }
    }

    /// <summary>
    /// Export worker performance report
    /// </summary>
    [HttpGet("worker-performance/export")]
    [RequireOwner]
    public async Task<IActionResult> ExportWorkerPerformance(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            var performance = await _dbContext.Shifts
                .Where(s => s.TenantId == tenantId.Value)
                .Where(s => s.ShiftDate >= from && s.ShiftDate <= to)
                .Include(s => s.Worker)
                .Include(s => s.FuelSales)
                .GroupBy(s => new { s.WorkerId, s.Worker!.FullName })
                .Select(g => new
                {
                    WorkerName = g.Key.FullName,
                    TotalShifts = g.Count(),
                    TotalSales = g.Sum(s => s.FuelSales.Count(fs => !fs.IsVoided)),
                    TotalAmount = g.Sum(s => s.TotalSales),
                    TotalQuantity = g.Sum(s => s.FuelSales.Where(fs => !fs.IsVoided).Sum(fs => fs.Quantity)),
                    TotalVariance = g.Sum(s => s.Variance)
                })
                .OrderByDescending(w => w.TotalAmount)
                .ToListAsync();

            var headers = new[] { "Worker", "Shifts", "Sales Count", "Quantity (L)", "Total Amount", "Avg/Shift", "Variance" };
            var rows = performance.Select(w => new[]
            {
                w.WorkerName,
                w.TotalShifts.ToString(),
                w.TotalSales.ToString(),
                w.TotalQuantity.ToString("N2"),
                w.TotalAmount.ToString("N2"),
                (w.TotalShifts > 0 ? w.TotalAmount / w.TotalShifts : 0).ToString("N2"),
                w.TotalVariance.ToString("N2")
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Workers", performance.Count.ToString() },
                { "Total Sales", performance.Sum(w => w.TotalAmount).ToString("N2") },
                { "Total Variance", performance.Sum(w => w.TotalVariance).ToString("N2") }
            };

            return GenerateExport($"Worker Performance Report ({from:dd-MMM} to {to:dd-MMM})", headers, rows, summary, format, "worker-performance");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting worker performance report");
            return StatusCode(500, "Failed to export report");
        }
    }

    /// <summary>
    /// Export nozzle performance report
    /// </summary>
    [HttpGet("nozzle-performance/export")]
    [RequireManager]
    public async Task<IActionResult> ExportNozzlePerformance(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? machineId,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? today.AddDays(-30);
            var to = toDate ?? today;

            var query = _dbContext.FuelSales
                .Where(fs => fs.TenantId == tenantId.Value && !fs.IsVoided)
                .Where(fs => DateOnly.FromDateTime(fs.SaleTime) >= from && DateOnly.FromDateTime(fs.SaleTime) <= to)
                .Include(fs => fs.Nozzle)
                    .ThenInclude(n => n!.Machine)
                .Include(fs => fs.Nozzle)
                    .ThenInclude(n => n!.FuelType)
                .AsQueryable();

            if (machineId.HasValue)
                query = query.Where(fs => fs.Nozzle!.MachineId == machineId.Value);

            var performance = await query
                .GroupBy(fs => new
                {
                    fs.Nozzle!.NozzleNumber,
                    MachineName = fs.Nozzle.Machine!.MachineName,
                    FuelType = fs.Nozzle.FuelType!.FuelName
                })
                .Select(g => new
                {
                    NozzleNumber = g.Key.NozzleNumber,
                    MachineName = g.Key.MachineName,
                    FuelType = g.Key.FuelType,
                    TotalQuantity = g.Sum(fs => fs.Quantity),
                    TotalAmount = g.Sum(fs => fs.Amount),
                    SalesCount = g.Count()
                })
                .OrderByDescending(n => n.TotalAmount)
                .ToListAsync();

            var headers = new[] { "Nozzle", "Machine", "Fuel Type", "Quantity (L)", "Amount", "Sales Count" };
            var rows = performance.Select(n => new[]
            {
                n.NozzleNumber,
                n.MachineName,
                n.FuelType,
                n.TotalQuantity.ToString("N2"),
                n.TotalAmount.ToString("N2"),
                n.SalesCount.ToString()
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Nozzles", performance.Count.ToString() },
                { "Total Sales", performance.Sum(n => n.TotalAmount).ToString("N2") },
                { "Total Quantity", performance.Sum(n => n.TotalQuantity).ToString("N2") + " L" }
            };

            return GenerateExport($"Nozzle Performance Report ({from:dd-MMM} to {to:dd-MMM})", headers, rows, summary, format, "nozzle-performance");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting nozzle performance report");
            return StatusCode(500, "Failed to export report");
        }
    }

    private IActionResult GenerateExport(string title, string[] headers, List<string[]> rows, Dictionary<string, string> summary, string format, string fileName)
    {
        byte[] fileBytes;
        string contentType;
        string extension;

        switch (format.ToLower())
        {
            case "excel":
            case "xlsx":
                fileBytes = _exportService.GenerateExcel(title, headers, rows, summary);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                extension = "xlsx";
                break;

            case "csv":
                fileBytes = _exportService.GenerateCsv(headers, rows);
                contentType = "text/csv";
                extension = "csv";
                break;

            default: // pdf
                fileBytes = _exportService.GeneratePdf(title, headers, rows, summary);
                contentType = "application/pdf";
                extension = "pdf";
                break;
        }

        return File(fileBytes, contentType, $"{fileName}-{DateTime.Now:yyyyMMdd}.{extension}");
    }
}
