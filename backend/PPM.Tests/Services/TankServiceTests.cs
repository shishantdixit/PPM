using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PPM.Application.DTOs.Stock;
using PPM.Core.Entities;
using PPM.Infrastructure.Services;
using PPM.Tests.Helpers;

namespace PPM.Tests.Services;

public class TankServiceTests : IDisposable
{
    private readonly Mock<ILogger<TankService>> _loggerMock;

    public TankServiceTests()
    {
        _loggerMock = new Mock<ILogger<TankService>>();
    }

    public void Dispose()
    {
        // Cleanup if needed
    }

    #region Tank CRUD Tests

    [Fact]
    public async Task GetAllTanksAsync_ReturnsOnlyTenantTanks()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var tanks = await service.GetAllTanksAsync(TestDbContextFactory.TestTenantId);

        // Assert
        tanks.Should().NotBeNull();
        tanks.Should().HaveCount(2); // Petrol Tank 1 and Diesel Tank 1
        tanks.Should().OnlyContain(t => t.TenantId == TestDbContextFactory.TestTenantId);
    }

    [Fact]
    public async Task GetAllTanksAsync_ReturnsEmptyForNonExistentTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var tanks = await service.GetAllTanksAsync(Guid.NewGuid());

        // Assert
        tanks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTankByIdAsync_ReturnsTank_WhenTankExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var tank = await service.GetTankByIdAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestPetrolTankId);

        // Assert
        tank.Should().NotBeNull();
        tank!.TankId.Should().Be(TestDbContextFactory.TestPetrolTankId);
        tank.TankCode.Should().Be("PT001");
        tank.FuelTypeName.Should().Be("Petrol");
    }

    [Fact]
    public async Task GetTankByIdAsync_ReturnsNull_WhenTankNotExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var tank = await service.GetTankByIdAsync(TestDbContextFactory.TestTenantId, Guid.NewGuid());

        // Assert
        tank.Should().BeNull();
    }

    [Fact]
    public async Task GetTankByIdAsync_ReturnsNull_WhenTankBelongsToDifferentTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act - Try to access Tenant1's tank using Tenant2's ID
        var tank = await service.GetTankByIdAsync(TestDbContextFactory.TestTenant2Id, TestDbContextFactory.TestPetrolTankId);

        // Assert
        tank.Should().BeNull();
    }

    [Fact]
    public async Task CreateTankAsync_CreatesTank_WhenValidData()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateTankDto
        {
            TankName = "New Petrol Tank",
            TankCode = "PT002",
            FuelTypeId = TestDbContextFactory.TestPetrolId,
            Capacity = 20000,
            CurrentStock = 5000,
            MinimumLevel = 2000,
            Location = "East Side"
        };

        // Act
        var tank = await service.CreateTankAsync(TestDbContextFactory.TestTenantId, dto);

        // Assert
        tank.Should().NotBeNull();
        tank.TankName.Should().Be("New Petrol Tank");
        tank.TankCode.Should().Be("PT002");
        tank.Capacity.Should().Be(20000);
        tank.CurrentStock.Should().Be(5000);
        tank.IsActive.Should().BeTrue();

        // Verify persisted
        var persisted = await context.Tanks.FindAsync(tank.TankId);
        persisted.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTankAsync_ThrowsException_WhenFuelTypeNotFound()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateTankDto
        {
            TankName = "New Tank",
            TankCode = "NT001",
            FuelTypeId = Guid.NewGuid(), // Non-existent fuel type
            Capacity = 10000,
            CurrentStock = 0,
            MinimumLevel = 1000
        };

        // Act & Assert
        await service.Invoking(s => s.CreateTankAsync(TestDbContextFactory.TestTenantId, dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Fuel type not found");
    }

    [Fact]
    public async Task CreateTankAsync_ThrowsException_WhenTankCodeDuplicate()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateTankDto
        {
            TankName = "Duplicate Tank",
            TankCode = "PT001", // Already exists
            FuelTypeId = TestDbContextFactory.TestPetrolId,
            Capacity = 10000,
            CurrentStock = 0,
            MinimumLevel = 1000
        };

        // Act & Assert
        await service.Invoking(s => s.CreateTankAsync(TestDbContextFactory.TestTenantId, dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task UpdateTankAsync_UpdatesTank_WhenValidData()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new UpdateTankDto
        {
            TankName = "Updated Tank Name",
            Capacity = 15000,
            MinimumLevel = 3000,
            Location = "New Location"
        };

        // Act
        var tank = await service.UpdateTankAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestPetrolTankId, dto);

        // Assert
        tank.Should().NotBeNull();
        tank!.TankName.Should().Be("Updated Tank Name");
        tank.Capacity.Should().Be(15000);
        tank.MinimumLevel.Should().Be(3000);
        tank.Location.Should().Be("New Location");
    }

    [Fact]
    public async Task UpdateTankAsync_ReturnsNull_WhenTankNotFound()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new UpdateTankDto { TankName = "Updated" };

        // Act
        var tank = await service.UpdateTankAsync(TestDbContextFactory.TestTenantId, Guid.NewGuid(), dto);

        // Assert
        tank.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTankAsync_HardDeletes_WhenNoStockEntries()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var result = await service.DeleteTankAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestPetrolTankId);

        // Assert
        result.Should().BeTrue();
        var tank = await context.Tanks.FindAsync(TestDbContextFactory.TestPetrolTankId);
        tank.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTankAsync_SoftDeletes_WhenHasStockEntries()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        // Add a stock entry
        context.StockEntries.Add(new StockEntry
        {
            StockEntryId = Guid.NewGuid(),
            TenantId = TestDbContextFactory.TestTenantId,
            TankId = TestDbContextFactory.TestPetrolTankId,
            EntryType = StockEntryType.StockIn,
            Quantity = 1000,
            StockBefore = 5000,
            StockAfter = 6000,
            EntryDate = DateTime.UtcNow,
            RecordedById = TestDbContextFactory.TestOwnerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new TankService(context, _loggerMock.Object);

        // Act
        var result = await service.DeleteTankAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestPetrolTankId);

        // Assert
        result.Should().BeTrue();
        var tank = await context.Tanks.FindAsync(TestDbContextFactory.TestPetrolTankId);
        tank.Should().NotBeNull();
        tank!.IsActive.Should().BeFalse(); // Soft deleted
    }

    [Fact]
    public async Task DeleteTankAsync_ReturnsFalse_WhenTankNotFound()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var result = await service.DeleteTankAsync(TestDbContextFactory.TestTenantId, Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Tank Summary Tests

    [Fact]
    public async Task GetTankSummaryAsync_ReturnsCorrectSummary()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var summary = await service.GetTankSummaryAsync(TestDbContextFactory.TestTenantId);

        // Assert
        summary.Should().NotBeNull();
        summary.TotalTanks.Should().Be(2);
        summary.ActiveTanks.Should().Be(2);
        summary.TotalCapacity.Should().Be(25000); // 10000 + 15000
        summary.TotalCurrentStock.Should().Be(5500); // 5000 + 500
        summary.LowStockTanks.Should().Be(1); // Diesel tank has 500 < 2000 minimum
        summary.ByFuelType.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTankSummaryAsync_IdentifiesLowStockTanks()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        // Act
        var summary = await service.GetTankSummaryAsync(TestDbContextFactory.TestTenantId);

        // Assert
        // Diesel tank has CurrentStock=500, MinimumLevel=2000, so it's low stock
        summary.LowStockTanks.Should().Be(1);
    }

    #endregion

    #region Stock Entry Tests

    [Fact]
    public async Task RecordStockInAsync_RecordsStockIn_AndUpdatesTankStock()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateStockInDto
        {
            TankId = TestDbContextFactory.TestPetrolTankId,
            Quantity = 2000,
            EntryDate = DateTime.UtcNow,
            Vendor = "Test Vendor",
            UnitPrice = 1.50m,
            Reference = "INV-001"
        };

        // Act
        var entry = await service.RecordStockInAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestOwnerId, dto);

        // Assert
        entry.Should().NotBeNull();
        entry.EntryType.Should().Be(StockEntryType.StockIn);
        entry.Quantity.Should().Be(2000);
        entry.StockBefore.Should().Be(5000);
        entry.StockAfter.Should().Be(7000);
        entry.TotalAmount.Should().Be(3000); // 2000 * 1.50

        // Verify tank stock updated
        var tank = await context.Tanks.FindAsync(TestDbContextFactory.TestPetrolTankId);
        tank!.CurrentStock.Should().Be(7000);
    }

    [Fact]
    public async Task RecordStockInAsync_ThrowsException_WhenExceedsCapacity()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateStockInDto
        {
            TankId = TestDbContextFactory.TestPetrolTankId,
            Quantity = 10000, // Would exceed 10000 capacity
            EntryDate = DateTime.UtcNow
        };

        // Act & Assert
        await service.Invoking(s => s.RecordStockInAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestOwnerId, dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*exceed tank capacity*");
    }

    [Fact]
    public async Task RecordStockInAsync_ThrowsException_WhenTankNotFound()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateStockInDto
        {
            TankId = Guid.NewGuid(),
            Quantity = 1000,
            EntryDate = DateTime.UtcNow
        };

        // Act & Assert
        await service.Invoking(s => s.RecordStockInAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestOwnerId, dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Tank not found");
    }

    [Fact]
    public async Task RecordStockAdjustmentAsync_RecordsPositiveAdjustment()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateStockAdjustmentDto
        {
            TankId = TestDbContextFactory.TestPetrolTankId,
            Quantity = 500, // Positive adjustment
            EntryDate = DateTime.UtcNow,
            Notes = "Calibration correction"
        };

        // Act
        var entry = await service.RecordStockAdjustmentAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestOwnerId, dto);

        // Assert
        entry.Should().NotBeNull();
        entry.EntryType.Should().Be(StockEntryType.Adjustment);
        entry.Quantity.Should().Be(500);
        entry.StockAfter.Should().Be(5500);

        var tank = await context.Tanks.FindAsync(TestDbContextFactory.TestPetrolTankId);
        tank!.CurrentStock.Should().Be(5500);
    }

    [Fact]
    public async Task RecordStockAdjustmentAsync_RecordsNegativeAdjustment()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateStockAdjustmentDto
        {
            TankId = TestDbContextFactory.TestPetrolTankId,
            Quantity = -1000, // Negative adjustment
            EntryDate = DateTime.UtcNow,
            Notes = "Leakage found"
        };

        // Act
        var entry = await service.RecordStockAdjustmentAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestOwnerId, dto);

        // Assert
        entry.Should().NotBeNull();
        entry.Quantity.Should().Be(-1000);
        entry.StockAfter.Should().Be(4000);

        var tank = await context.Tanks.FindAsync(TestDbContextFactory.TestPetrolTankId);
        tank!.CurrentStock.Should().Be(4000);
    }

    [Fact]
    public async Task RecordStockAdjustmentAsync_ThrowsException_WhenResultsInNegativeStock()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateStockAdjustmentDto
        {
            TankId = TestDbContextFactory.TestPetrolTankId,
            Quantity = -10000, // Would result in negative stock
            EntryDate = DateTime.UtcNow
        };

        // Act & Assert
        await service.Invoking(s => s.RecordStockAdjustmentAsync(TestDbContextFactory.TestTenantId, TestDbContextFactory.TestOwnerId, dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*negative stock*");
    }

    [Fact]
    public async Task RecordStockOutFromSaleAsync_DeductsStock()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var shiftId = Guid.NewGuid();
        var saleId = Guid.NewGuid();

        // Act
        var entry = await service.RecordStockOutFromSaleAsync(
            TestDbContextFactory.TestTenantId,
            TestDbContextFactory.TestPetrolTankId,
            shiftId,
            saleId,
            100,
            TestDbContextFactory.TestWorkerId);

        // Assert
        entry.Should().NotBeNull();
        entry!.EntryType.Should().Be(StockEntryType.StockOut);
        entry.Quantity.Should().Be(-100); // Negative for stock out
        entry.StockBefore.Should().Be(5000);
        entry.StockAfter.Should().Be(4900);
        entry.ShiftId.Should().Be(shiftId);
        entry.FuelSaleId.Should().Be(saleId);

        var tank = await context.Tanks.FindAsync(TestDbContextFactory.TestPetrolTankId);
        tank!.CurrentStock.Should().Be(4900);
    }

    [Fact]
    public async Task GetStockHistoryAsync_ReturnsFilteredResults()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        // Add some stock entries
        var entries = new[]
        {
            new StockEntry
            {
                StockEntryId = Guid.NewGuid(),
                TenantId = TestDbContextFactory.TestTenantId,
                TankId = TestDbContextFactory.TestPetrolTankId,
                EntryType = StockEntryType.StockIn,
                Quantity = 1000,
                StockBefore = 5000,
                StockAfter = 6000,
                EntryDate = DateTime.UtcNow.AddDays(-2),
                RecordedById = TestDbContextFactory.TestOwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new StockEntry
            {
                StockEntryId = Guid.NewGuid(),
                TenantId = TestDbContextFactory.TestTenantId,
                TankId = TestDbContextFactory.TestPetrolTankId,
                EntryType = StockEntryType.StockOut,
                Quantity = -500,
                StockBefore = 6000,
                StockAfter = 5500,
                EntryDate = DateTime.UtcNow.AddDays(-1),
                RecordedById = TestDbContextFactory.TestWorkerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new StockEntry
            {
                StockEntryId = Guid.NewGuid(),
                TenantId = TestDbContextFactory.TestTenantId,
                TankId = TestDbContextFactory.TestDieselTankId, // Different tank
                EntryType = StockEntryType.StockIn,
                Quantity = 2000,
                StockBefore = 500,
                StockAfter = 2500,
                EntryDate = DateTime.UtcNow,
                RecordedById = TestDbContextFactory.TestOwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.StockEntries.AddRange(entries);
        await context.SaveChangesAsync();

        var service = new TankService(context, _loggerMock.Object);

        // Act - Filter by tank
        var filter = new StockHistoryFilterDto
        {
            TankId = TestDbContextFactory.TestPetrolTankId,
            Page = 1,
            PageSize = 10
        };
        var result = await service.GetStockHistoryAsync(TestDbContextFactory.TestTenantId, filter);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(e => e.TankId == TestDbContextFactory.TestPetrolTankId);
    }

    [Fact]
    public async Task GetStockHistoryAsync_FiltersByEntryType()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        context.StockEntries.AddRange(new[]
        {
            new StockEntry
            {
                StockEntryId = Guid.NewGuid(),
                TenantId = TestDbContextFactory.TestTenantId,
                TankId = TestDbContextFactory.TestPetrolTankId,
                EntryType = StockEntryType.StockIn,
                Quantity = 1000,
                StockBefore = 5000,
                StockAfter = 6000,
                EntryDate = DateTime.UtcNow,
                RecordedById = TestDbContextFactory.TestOwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new StockEntry
            {
                StockEntryId = Guid.NewGuid(),
                TenantId = TestDbContextFactory.TestTenantId,
                TankId = TestDbContextFactory.TestPetrolTankId,
                EntryType = StockEntryType.Adjustment,
                Quantity = -200,
                StockBefore = 6000,
                StockAfter = 5800,
                EntryDate = DateTime.UtcNow,
                RecordedById = TestDbContextFactory.TestOwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        });
        await context.SaveChangesAsync();

        var service = new TankService(context, _loggerMock.Object);

        // Act
        var filter = new StockHistoryFilterDto
        {
            EntryType = StockEntryType.StockIn,
            Page = 1,
            PageSize = 10
        };
        var result = await service.GetStockHistoryAsync(TestDbContextFactory.TestTenantId, filter);

        // Assert
        result.Items.Should().OnlyContain(e => e.EntryType == StockEntryType.StockIn);
    }

    #endregion

    #region Multi-Tenant Isolation Tests

    [Fact]
    public async Task GetAllTanksAsync_EnforcesTenantIsolation()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        // Add tank for tenant 2
        context.Tanks.Add(new Tank
        {
            TankId = Guid.NewGuid(),
            TenantId = TestDbContextFactory.TestTenant2Id,
            TankName = "Tenant 2 Tank",
            TankCode = "T2-001",
            FuelTypeId = TestDbContextFactory.TestPetrolId, // Would need tenant2's fuel type in real scenario
            Capacity = 5000,
            CurrentStock = 1000,
            MinimumLevel = 500,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new TankService(context, _loggerMock.Object);

        // Act
        var tenant1Tanks = await service.GetAllTanksAsync(TestDbContextFactory.TestTenantId);
        var tenant2Tanks = await service.GetAllTanksAsync(TestDbContextFactory.TestTenant2Id);

        // Assert
        tenant1Tanks.Should().HaveCount(2);
        tenant1Tanks.Should().OnlyContain(t => t.TenantId == TestDbContextFactory.TestTenantId);

        tenant2Tanks.Should().HaveCount(1);
        tenant2Tanks.Should().OnlyContain(t => t.TenantId == TestDbContextFactory.TestTenant2Id);
    }

    [Fact]
    public async Task CreateTankAsync_CannotUseFuelTypeFromOtherTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new TankService(context, _loggerMock.Object);

        var dto = new CreateTankDto
        {
            TankName = "Cross-Tenant Tank",
            TankCode = "CT001",
            FuelTypeId = TestDbContextFactory.TestPetrolId, // Tenant1's fuel type
            Capacity = 5000,
            CurrentStock = 1000,
            MinimumLevel = 500
        };

        // Act & Assert - Tenant 2 tries to use Tenant 1's fuel type
        await service.Invoking(s => s.CreateTankAsync(TestDbContextFactory.TestTenant2Id, dto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Fuel type not found");
    }

    #endregion
}
