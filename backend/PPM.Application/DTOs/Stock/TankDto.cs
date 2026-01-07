using PPM.Core.Entities;

namespace PPM.Application.DTOs.Stock;

public class TankDto
{
    public Guid TankId { get; set; }
    public Guid TenantId { get; set; }
    public string TankName { get; set; } = string.Empty;
    public string TankCode { get; set; } = string.Empty;
    public Guid FuelTypeId { get; set; }
    public string FuelTypeName { get; set; } = string.Empty;
    public string FuelTypeCode { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumLevel { get; set; }
    public decimal AvailablePercentage { get; set; }
    public bool IsLowStock { get; set; }
    public string? Location { get; set; }
    public DateOnly? InstallationDate { get; set; }
    public DateOnly? LastCalibrationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTankDto
{
    public string TankName { get; set; } = string.Empty;
    public string TankCode { get; set; } = string.Empty;
    public Guid FuelTypeId { get; set; }
    public decimal Capacity { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumLevel { get; set; }
    public string? Location { get; set; }
    public DateOnly? InstallationDate { get; set; }
    public DateOnly? LastCalibrationDate { get; set; }
}

public class UpdateTankDto
{
    public string? TankName { get; set; }
    public Guid? FuelTypeId { get; set; }
    public decimal? Capacity { get; set; }
    public decimal? MinimumLevel { get; set; }
    public string? Location { get; set; }
    public DateOnly? LastCalibrationDate { get; set; }
    public bool? IsActive { get; set; }
}

public class TankSummaryDto
{
    public int TotalTanks { get; set; }
    public int ActiveTanks { get; set; }
    public int LowStockTanks { get; set; }
    public decimal TotalCapacity { get; set; }
    public decimal TotalCurrentStock { get; set; }
    public List<TankStockByFuelTypeDto> ByFuelType { get; set; } = new();
}

public class TankStockByFuelTypeDto
{
    public Guid FuelTypeId { get; set; }
    public string FuelTypeName { get; set; } = string.Empty;
    public int TankCount { get; set; }
    public decimal TotalCapacity { get; set; }
    public decimal TotalStock { get; set; }
    public decimal AvailablePercentage { get; set; }
}
