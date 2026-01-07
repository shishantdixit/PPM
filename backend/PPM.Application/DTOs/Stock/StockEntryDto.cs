using PPM.Core.Entities;

namespace PPM.Application.DTOs.Stock;

public class StockEntryDto
{
    public Guid StockEntryId { get; set; }
    public Guid TenantId { get; set; }
    public Guid TankId { get; set; }
    public string TankName { get; set; } = string.Empty;
    public string TankCode { get; set; } = string.Empty;
    public string FuelTypeName { get; set; } = string.Empty;
    public StockEntryType EntryType { get; set; }
    public string EntryTypeName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal StockBefore { get; set; }
    public decimal StockAfter { get; set; }
    public DateTime EntryDate { get; set; }
    public string? Reference { get; set; }
    public string? Vendor { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Notes { get; set; }
    public Guid? ShiftId { get; set; }
    public Guid? FuelSaleId { get; set; }
    public Guid RecordedById { get; set; }
    public string RecordedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateStockInDto
{
    public Guid TankId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime EntryDate { get; set; }
    public string? Reference { get; set; }
    public string? Vendor { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Notes { get; set; }
}

public class CreateStockAdjustmentDto
{
    public Guid TankId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime EntryDate { get; set; }
    public string? Notes { get; set; }
}

public class StockHistoryFilterDto
{
    public Guid? TankId { get; set; }
    public StockEntryType? EntryType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class StockSummaryDto
{
    public Guid TankId { get; set; }
    public string TankName { get; set; } = string.Empty;
    public string TankCode { get; set; } = string.Empty;
    public string FuelTypeName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal TotalStockIn { get; set; }
    public decimal TotalStockOut { get; set; }
    public decimal TotalAdjustments { get; set; }
    public int StockInCount { get; set; }
    public int StockOutCount { get; set; }
    public int AdjustmentCount { get; set; }
    public DateTime? LastStockInDate { get; set; }
    public DateTime? LastStockOutDate { get; set; }
}

public class StockMovementReportDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal OpeningStock { get; set; }
    public decimal TotalReceived { get; set; }
    public decimal TotalSold { get; set; }
    public decimal TotalAdjustments { get; set; }
    public decimal ClosingStock { get; set; }
    public decimal Variance { get; set; }
    public List<DailyStockMovementDto> DailyMovements { get; set; } = new();
}

public class DailyStockMovementDto
{
    public DateOnly Date { get; set; }
    public decimal OpeningStock { get; set; }
    public decimal StockIn { get; set; }
    public decimal StockOut { get; set; }
    public decimal Adjustments { get; set; }
    public decimal ClosingStock { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
