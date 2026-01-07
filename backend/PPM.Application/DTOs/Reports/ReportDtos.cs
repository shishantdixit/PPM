using PPM.Core.Entities;

namespace PPM.Application.DTOs.Reports;

// Daily Sales Summary
public class DailySalesSummaryDto
{
    public DateOnly Date { get; set; }
    public int TotalShifts { get; set; }
    public int TotalSales { get; set; }
    public int VoidedSales { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CashAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal DigitalAmount { get; set; }
    public List<FuelSalesByTypeDto> SalesByFuelType { get; set; } = new();
}

public class FuelSalesByTypeDto
{
    public Guid FuelTypeId { get; set; }
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal AverageRate { get; set; }
}

// Shift Summary Report
public class ShiftSummaryReportDto
{
    public Guid ShiftId { get; set; }
    public DateOnly ShiftDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public ShiftStatus Status { get; set; }
    public int SalesCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalSales { get; set; }
    public decimal CashCollected { get; set; }
    public decimal CreditSales { get; set; }
    public decimal DigitalPayments { get; set; }
    public decimal Variance { get; set; }
}

// Sales Trend Report (for charts)
public class SalesTrendDto
{
    public DateOnly Date { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQuantity { get; set; }
    public int SalesCount { get; set; }
}

// Fuel Type Performance Report
public class FuelTypePerformanceDto
{
    public Guid FuelTypeId { get; set; }
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageRate { get; set; }
    public int SalesCount { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

// Worker Performance Report
public class WorkerPerformanceDto
{
    public Guid UserId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public int TotalShifts { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal AverageShiftSales { get; set; }
    public decimal TotalVariance { get; set; }
}

// Nozzle Performance Report
public class NozzlePerformanceDto
{
    public Guid NozzleId { get; set; }
    public string NozzleNumber { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public int SalesCount { get; set; }
}

// Dashboard Summary (for main dashboard)
public class DashboardSummaryDto
{
    public DateOnly Date { get; set; }

    // Today's Stats
    public decimal TodaySales { get; set; }
    public decimal TodayQuantity { get; set; }
    public int TodaySalesCount { get; set; }
    public int ActiveShifts { get; set; }

    // This Week Stats
    public decimal WeekSales { get; set; }
    public decimal WeekQuantity { get; set; }

    // This Month Stats
    public decimal MonthSales { get; set; }
    public decimal MonthQuantity { get; set; }

    // Comparison with previous period
    public decimal SalesChangePercent { get; set; } // vs yesterday
    public decimal WeekChangePercent { get; set; }  // vs last week
    public decimal MonthChangePercent { get; set; } // vs last month

    // Top performers
    public List<FuelTypePerformanceDto> TopFuelTypes { get; set; } = new();
    public List<SalesTrendDto> Last7DaysTrend { get; set; } = new();
}

// Report Filter Parameters
public class ReportFilterDto
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public Guid? WorkerId { get; set; }
    public Guid? FuelTypeId { get; set; }
    public Guid? MachineId { get; set; }
}
