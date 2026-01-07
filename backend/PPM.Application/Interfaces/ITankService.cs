using PPM.Application.DTOs.Stock;

namespace PPM.Application.Interfaces;

/// <summary>
/// Service for managing tanks and stock operations
/// </summary>
public interface ITankService
{
    // Tank Operations
    Task<List<TankDto>> GetAllTanksAsync(Guid tenantId);
    Task<TankDto?> GetTankByIdAsync(Guid tenantId, Guid tankId);
    Task<TankDto> CreateTankAsync(Guid tenantId, CreateTankDto dto);
    Task<TankDto?> UpdateTankAsync(Guid tenantId, Guid tankId, UpdateTankDto dto);
    Task<bool> DeleteTankAsync(Guid tenantId, Guid tankId);
    Task<TankSummaryDto> GetTankSummaryAsync(Guid tenantId);

    // Stock Entry Operations
    Task<PagedResult<StockEntryDto>> GetStockHistoryAsync(Guid tenantId, StockHistoryFilterDto filter);
    Task<StockEntryDto> RecordStockInAsync(Guid tenantId, Guid recordedById, CreateStockInDto dto);
    Task<StockEntryDto> RecordStockAdjustmentAsync(Guid tenantId, Guid recordedById, CreateStockAdjustmentDto dto);
    Task<StockSummaryDto?> GetTankStockSummaryAsync(Guid tenantId, Guid tankId, DateTime? fromDate, DateTime? toDate);
    Task<StockMovementReportDto?> GetStockMovementReportAsync(Guid tenantId, Guid tankId, DateTime fromDate, DateTime toDate);

    // Internal operation for auto-deduction from sales
    Task<StockEntryDto?> RecordStockOutFromSaleAsync(Guid tenantId, Guid tankId, Guid shiftId, Guid fuelSaleId, decimal quantity, Guid recordedById);
}
