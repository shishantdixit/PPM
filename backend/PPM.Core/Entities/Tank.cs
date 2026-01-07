using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Represents a fuel storage tank at the petrol pump
/// </summary>
public class Tank : TenantEntity
{
    public Guid TankId { get; set; }
    public string TankName { get; set; } = string.Empty;        // Tank 1, Tank 2, etc.
    public string TankCode { get; set; } = string.Empty;        // T001, T002, etc.
    public Guid FuelTypeId { get; set; }                        // What fuel type this tank stores
    public decimal Capacity { get; set; }                       // Maximum capacity in liters
    public decimal CurrentStock { get; set; }                   // Current stock level in liters
    public decimal MinimumLevel { get; set; }                   // Low stock alert threshold
    public string? Location { get; set; }                       // Physical location description
    public DateOnly? InstallationDate { get; set; }
    public DateOnly? LastCalibrationDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public FuelType? FuelType { get; set; }
    public ICollection<StockEntry> StockEntries { get; set; } = new List<StockEntry>();
}
