using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPM.Application.DTOs.Stock;
using PPM.Application.Interfaces;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.Infrastructure.Services;

/// <summary>
/// Service for managing tanks and stock operations
/// </summary>
public class TankService : ITankService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<TankService> _logger;

    public TankService(ApplicationDbContext dbContext, ILogger<TankService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    #region Tank Operations

    public async Task<List<TankDto>> GetAllTanksAsync(Guid tenantId)
    {
        return await _dbContext.Tanks
            .Where(t => t.TenantId == tenantId)
            .Include(t => t.FuelType)
            .OrderBy(t => t.TankCode)
            .Select(t => MapToTankDto(t))
            .ToListAsync();
    }

    public async Task<TankDto?> GetTankByIdAsync(Guid tenantId, Guid tankId)
    {
        var tank = await _dbContext.Tanks
            .Where(t => t.TenantId == tenantId && t.TankId == tankId)
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync();

        return tank != null ? MapToTankDto(tank) : null;
    }

    public async Task<TankDto> CreateTankAsync(Guid tenantId, CreateTankDto dto)
    {
        // Verify fuel type exists
        var fuelType = await _dbContext.FuelTypes
            .FirstOrDefaultAsync(f => f.TenantId == tenantId && f.FuelTypeId == dto.FuelTypeId);

        if (fuelType == null)
        {
            throw new InvalidOperationException("Fuel type not found");
        }

        // Check for duplicate tank code
        var existing = await _dbContext.Tanks
            .AnyAsync(t => t.TenantId == tenantId && t.TankCode == dto.TankCode);

        if (existing)
        {
            throw new InvalidOperationException($"Tank with code '{dto.TankCode}' already exists");
        }

        var tank = new Tank
        {
            TankId = Guid.NewGuid(),
            TenantId = tenantId,
            TankName = dto.TankName,
            TankCode = dto.TankCode,
            FuelTypeId = dto.FuelTypeId,
            Capacity = dto.Capacity,
            CurrentStock = dto.CurrentStock,
            MinimumLevel = dto.MinimumLevel,
            Location = dto.Location,
            InstallationDate = dto.InstallationDate,
            LastCalibrationDate = dto.LastCalibrationDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Tanks.Add(tank);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Created tank {TankCode} for tenant {TenantId}", dto.TankCode, tenantId);

        tank.FuelType = fuelType;
        return MapToTankDto(tank);
    }

    public async Task<TankDto?> UpdateTankAsync(Guid tenantId, Guid tankId, UpdateTankDto dto)
    {
        var tank = await _dbContext.Tanks
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TankId == tankId);

        if (tank == null)
        {
            return null;
        }

        if (dto.TankName != null) tank.TankName = dto.TankName;
        if (dto.FuelTypeId.HasValue)
        {
            var fuelType = await _dbContext.FuelTypes
                .FirstOrDefaultAsync(f => f.TenantId == tenantId && f.FuelTypeId == dto.FuelTypeId.Value);
            if (fuelType == null)
            {
                throw new InvalidOperationException("Fuel type not found");
            }
            tank.FuelTypeId = dto.FuelTypeId.Value;
            tank.FuelType = fuelType;
        }
        if (dto.Capacity.HasValue) tank.Capacity = dto.Capacity.Value;
        if (dto.MinimumLevel.HasValue) tank.MinimumLevel = dto.MinimumLevel.Value;
        if (dto.Location != null) tank.Location = dto.Location;
        if (dto.LastCalibrationDate.HasValue) tank.LastCalibrationDate = dto.LastCalibrationDate.Value;
        if (dto.IsActive.HasValue) tank.IsActive = dto.IsActive.Value;

        tank.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Updated tank {TankId} for tenant {TenantId}", tankId, tenantId);

        return MapToTankDto(tank);
    }

    public async Task<bool> DeleteTankAsync(Guid tenantId, Guid tankId)
    {
        var tank = await _dbContext.Tanks
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TankId == tankId);

        if (tank == null)
        {
            return false;
        }

        // Check if there are any stock entries
        var hasStockEntries = await _dbContext.StockEntries
            .AnyAsync(s => s.TankId == tankId);

        if (hasStockEntries)
        {
            // Soft delete if there are stock entries
            tank.IsActive = false;
            tank.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Soft deleted tank {TankId} for tenant {TenantId}", tankId, tenantId);
        }
        else
        {
            // Hard delete if no stock entries
            _dbContext.Tanks.Remove(tank);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Deleted tank {TankId} for tenant {TenantId}", tankId, tenantId);
        }

        return true;
    }

    public async Task<TankSummaryDto> GetTankSummaryAsync(Guid tenantId)
    {
        var tanks = await _dbContext.Tanks
            .Where(t => t.TenantId == tenantId)
            .Include(t => t.FuelType)
            .ToListAsync();

        var activeTanks = tanks.Where(t => t.IsActive).ToList();

        return new TankSummaryDto
        {
            TotalTanks = tanks.Count,
            ActiveTanks = activeTanks.Count,
            LowStockTanks = activeTanks.Count(t => t.CurrentStock <= t.MinimumLevel),
            TotalCapacity = activeTanks.Sum(t => t.Capacity),
            TotalCurrentStock = activeTanks.Sum(t => t.CurrentStock),
            ByFuelType = activeTanks
                .GroupBy(t => new { t.FuelTypeId, t.FuelType?.FuelName })
                .Select(g => new TankStockByFuelTypeDto
                {
                    FuelTypeId = g.Key.FuelTypeId,
                    FuelTypeName = g.Key.FuelName ?? "Unknown",
                    TankCount = g.Count(),
                    TotalCapacity = g.Sum(t => t.Capacity),
                    TotalStock = g.Sum(t => t.CurrentStock),
                    AvailablePercentage = g.Sum(t => t.Capacity) > 0
                        ? Math.Round(g.Sum(t => t.CurrentStock) / g.Sum(t => t.Capacity) * 100, 2)
                        : 0
                })
                .ToList()
        };
    }

    #endregion

    #region Stock Entry Operations

    public async Task<PagedResult<StockEntryDto>> GetStockHistoryAsync(Guid tenantId, StockHistoryFilterDto filter)
    {
        var query = _dbContext.StockEntries
            .Where(s => s.TenantId == tenantId)
            .Include(s => s.Tank)
                .ThenInclude(t => t!.FuelType)
            .Include(s => s.RecordedBy)
            .AsQueryable();

        if (filter.TankId.HasValue)
        {
            query = query.Where(s => s.TankId == filter.TankId.Value);
        }

        if (filter.EntryType.HasValue)
        {
            query = query.Where(s => s.EntryType == filter.EntryType.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(s => s.EntryDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(s => s.EntryDate <= filter.ToDate.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.EntryDate)
            .ThenByDescending(s => s.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s => MapToStockEntryDto(s))
            .ToListAsync();

        return new PagedResult<StockEntryDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<StockEntryDto> RecordStockInAsync(Guid tenantId, Guid recordedById, CreateStockInDto dto)
    {
        var tank = await _dbContext.Tanks
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TankId == dto.TankId);

        if (tank == null)
        {
            throw new InvalidOperationException("Tank not found");
        }

        var stockBefore = tank.CurrentStock;
        var stockAfter = stockBefore + dto.Quantity;

        if (stockAfter > tank.Capacity)
        {
            throw new InvalidOperationException($"Stock ({stockAfter:N2}) would exceed tank capacity ({tank.Capacity:N2})");
        }

        var entry = new StockEntry
        {
            StockEntryId = Guid.NewGuid(),
            TenantId = tenantId,
            TankId = dto.TankId,
            EntryType = StockEntryType.StockIn,
            Quantity = dto.Quantity,
            StockBefore = stockBefore,
            StockAfter = stockAfter,
            EntryDate = dto.EntryDate,
            Reference = dto.Reference,
            Vendor = dto.Vendor,
            UnitPrice = dto.UnitPrice,
            TotalAmount = dto.UnitPrice.HasValue ? dto.UnitPrice.Value * dto.Quantity : null,
            Notes = dto.Notes,
            RecordedById = recordedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        tank.CurrentStock = stockAfter;
        tank.UpdatedAt = DateTime.UtcNow;

        _dbContext.StockEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Recorded stock in of {Quantity} for tank {TankId}", dto.Quantity, dto.TankId);

        entry.Tank = tank;
        var recorder = await _dbContext.Users.FindAsync(recordedById);
        entry.RecordedBy = recorder;

        return MapToStockEntryDto(entry);
    }

    public async Task<StockEntryDto> RecordStockAdjustmentAsync(Guid tenantId, Guid recordedById, CreateStockAdjustmentDto dto)
    {
        var tank = await _dbContext.Tanks
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TankId == dto.TankId);

        if (tank == null)
        {
            throw new InvalidOperationException("Tank not found");
        }

        var stockBefore = tank.CurrentStock;
        var stockAfter = stockBefore + dto.Quantity; // Quantity can be negative for reduction

        if (stockAfter < 0)
        {
            throw new InvalidOperationException($"Stock adjustment would result in negative stock ({stockAfter:N2})");
        }

        if (stockAfter > tank.Capacity)
        {
            throw new InvalidOperationException($"Stock ({stockAfter:N2}) would exceed tank capacity ({tank.Capacity:N2})");
        }

        var entry = new StockEntry
        {
            StockEntryId = Guid.NewGuid(),
            TenantId = tenantId,
            TankId = dto.TankId,
            EntryType = StockEntryType.Adjustment,
            Quantity = dto.Quantity,
            StockBefore = stockBefore,
            StockAfter = stockAfter,
            EntryDate = dto.EntryDate,
            Notes = dto.Notes,
            RecordedById = recordedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        tank.CurrentStock = stockAfter;
        tank.UpdatedAt = DateTime.UtcNow;

        _dbContext.StockEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Recorded stock adjustment of {Quantity} for tank {TankId}", dto.Quantity, dto.TankId);

        entry.Tank = tank;
        var recorder = await _dbContext.Users.FindAsync(recordedById);
        entry.RecordedBy = recorder;

        return MapToStockEntryDto(entry);
    }

    public async Task<StockEntryDto?> RecordStockOutFromSaleAsync(Guid tenantId, Guid tankId, Guid shiftId, Guid fuelSaleId, decimal quantity, Guid recordedById)
    {
        var tank = await _dbContext.Tanks
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TankId == tankId);

        if (tank == null)
        {
            _logger.LogWarning("Tank {TankId} not found for stock out from sale", tankId);
            return null;
        }

        var stockBefore = tank.CurrentStock;
        var stockAfter = stockBefore - quantity;

        // Allow negative stock for tracking purposes but log warning
        if (stockAfter < 0)
        {
            _logger.LogWarning("Stock out would result in negative stock for tank {TankId}. Before: {Before}, After: {After}", tankId, stockBefore, stockAfter);
        }

        var entry = new StockEntry
        {
            StockEntryId = Guid.NewGuid(),
            TenantId = tenantId,
            TankId = tankId,
            EntryType = StockEntryType.StockOut,
            Quantity = -quantity, // Negative for stock out
            StockBefore = stockBefore,
            StockAfter = stockAfter,
            EntryDate = DateTime.UtcNow,
            ShiftId = shiftId,
            FuelSaleId = fuelSaleId,
            RecordedById = recordedById,
            Notes = "Auto-deducted from fuel sale",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        tank.CurrentStock = stockAfter;
        tank.UpdatedAt = DateTime.UtcNow;

        _dbContext.StockEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Recorded stock out of {Quantity} for tank {TankId} from sale {SaleId}", quantity, tankId, fuelSaleId);

        entry.Tank = tank;
        return MapToStockEntryDto(entry);
    }

    public async Task<StockSummaryDto?> GetTankStockSummaryAsync(Guid tenantId, Guid tankId, DateTime? fromDate, DateTime? toDate)
    {
        var tank = await _dbContext.Tanks
            .Include(t => t.FuelType)
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TankId == tankId);

        if (tank == null)
        {
            return null;
        }

        var query = _dbContext.StockEntries
            .Where(s => s.TenantId == tenantId && s.TankId == tankId);

        if (fromDate.HasValue)
        {
            query = query.Where(s => s.EntryDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(s => s.EntryDate <= toDate.Value);
        }

        var entries = await query.ToListAsync();

        return new StockSummaryDto
        {
            TankId = tank.TankId,
            TankName = tank.TankName,
            TankCode = tank.TankCode,
            FuelTypeName = tank.FuelType?.FuelName ?? "Unknown",
            CurrentStock = tank.CurrentStock,
            TotalStockIn = entries.Where(e => e.EntryType == StockEntryType.StockIn).Sum(e => e.Quantity),
            TotalStockOut = entries.Where(e => e.EntryType == StockEntryType.StockOut).Sum(e => Math.Abs(e.Quantity)),
            TotalAdjustments = entries.Where(e => e.EntryType == StockEntryType.Adjustment).Sum(e => e.Quantity),
            StockInCount = entries.Count(e => e.EntryType == StockEntryType.StockIn),
            StockOutCount = entries.Count(e => e.EntryType == StockEntryType.StockOut),
            AdjustmentCount = entries.Count(e => e.EntryType == StockEntryType.Adjustment),
            LastStockInDate = entries.Where(e => e.EntryType == StockEntryType.StockIn).MaxBy(e => e.EntryDate)?.EntryDate,
            LastStockOutDate = entries.Where(e => e.EntryType == StockEntryType.StockOut).MaxBy(e => e.EntryDate)?.EntryDate
        };
    }

    public async Task<StockMovementReportDto?> GetStockMovementReportAsync(Guid tenantId, Guid tankId, DateTime fromDate, DateTime toDate)
    {
        var tank = await _dbContext.Tanks
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.TankId == tankId);

        if (tank == null)
        {
            return null;
        }

        var entries = await _dbContext.StockEntries
            .Where(s => s.TenantId == tenantId && s.TankId == tankId && s.EntryDate >= fromDate && s.EntryDate <= toDate)
            .OrderBy(s => s.EntryDate)
            .ToListAsync();

        // Get opening stock (last entry before period or tank's initial stock)
        var openingEntry = await _dbContext.StockEntries
            .Where(s => s.TenantId == tenantId && s.TankId == tankId && s.EntryDate < fromDate)
            .OrderByDescending(s => s.EntryDate)
            .ThenByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        var openingStock = openingEntry?.StockAfter ?? 0;

        var report = new StockMovementReportDto
        {
            PeriodStart = fromDate,
            PeriodEnd = toDate,
            OpeningStock = openingStock,
            TotalReceived = entries.Where(e => e.EntryType == StockEntryType.StockIn).Sum(e => e.Quantity),
            TotalSold = entries.Where(e => e.EntryType == StockEntryType.StockOut).Sum(e => Math.Abs(e.Quantity)),
            TotalAdjustments = entries.Where(e => e.EntryType == StockEntryType.Adjustment).Sum(e => e.Quantity),
            ClosingStock = entries.Any() ? entries.Last().StockAfter : openingStock
        };

        report.Variance = report.OpeningStock + report.TotalReceived - report.TotalSold + report.TotalAdjustments - report.ClosingStock;

        // Generate daily movements
        var dailyGroups = entries
            .GroupBy(e => DateOnly.FromDateTime(e.EntryDate))
            .OrderBy(g => g.Key);

        var runningStock = openingStock;
        foreach (var day in dailyGroups)
        {
            var dayOpeningStock = runningStock;
            var stockIn = day.Where(e => e.EntryType == StockEntryType.StockIn).Sum(e => e.Quantity);
            var stockOut = day.Where(e => e.EntryType == StockEntryType.StockOut).Sum(e => Math.Abs(e.Quantity));
            var adjustments = day.Where(e => e.EntryType == StockEntryType.Adjustment).Sum(e => e.Quantity);
            var closingStock = day.Last().StockAfter;

            report.DailyMovements.Add(new DailyStockMovementDto
            {
                Date = day.Key,
                OpeningStock = dayOpeningStock,
                StockIn = stockIn,
                StockOut = stockOut,
                Adjustments = adjustments,
                ClosingStock = closingStock
            });

            runningStock = closingStock;
        }

        return report;
    }

    #endregion

    #region Private Helpers

    private static TankDto MapToTankDto(Tank tank)
    {
        var availablePercentage = tank.Capacity > 0
            ? Math.Round(tank.CurrentStock / tank.Capacity * 100, 2)
            : 0;

        return new TankDto
        {
            TankId = tank.TankId,
            TenantId = tank.TenantId,
            TankName = tank.TankName,
            TankCode = tank.TankCode,
            FuelTypeId = tank.FuelTypeId,
            FuelTypeName = tank.FuelType?.FuelName ?? "Unknown",
            FuelTypeCode = tank.FuelType?.FuelCode ?? "Unknown",
            Capacity = tank.Capacity,
            CurrentStock = tank.CurrentStock,
            MinimumLevel = tank.MinimumLevel,
            AvailablePercentage = availablePercentage,
            IsLowStock = tank.CurrentStock <= tank.MinimumLevel,
            Location = tank.Location,
            InstallationDate = tank.InstallationDate,
            LastCalibrationDate = tank.LastCalibrationDate,
            IsActive = tank.IsActive,
            CreatedAt = tank.CreatedAt,
            UpdatedAt = tank.UpdatedAt
        };
    }

    private static StockEntryDto MapToStockEntryDto(StockEntry entry)
    {
        return new StockEntryDto
        {
            StockEntryId = entry.StockEntryId,
            TenantId = entry.TenantId,
            TankId = entry.TankId,
            TankName = entry.Tank?.TankName ?? "Unknown",
            TankCode = entry.Tank?.TankCode ?? "Unknown",
            FuelTypeName = entry.Tank?.FuelType?.FuelName ?? "Unknown",
            EntryType = entry.EntryType,
            EntryTypeName = entry.EntryType.ToString(),
            Quantity = entry.Quantity,
            StockBefore = entry.StockBefore,
            StockAfter = entry.StockAfter,
            EntryDate = entry.EntryDate,
            Reference = entry.Reference,
            Vendor = entry.Vendor,
            UnitPrice = entry.UnitPrice,
            TotalAmount = entry.TotalAmount,
            Notes = entry.Notes,
            ShiftId = entry.ShiftId,
            FuelSaleId = entry.FuelSaleId,
            RecordedById = entry.RecordedById,
            RecordedByName = entry.RecordedBy?.FullName ?? "Unknown",
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };
    }

    #endregion
}
