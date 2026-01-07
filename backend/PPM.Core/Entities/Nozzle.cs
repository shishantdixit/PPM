using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Represents a nozzle on a fuel dispenser machine
/// Each nozzle dispenses one type of fuel
/// </summary>
public class Nozzle : TenantEntity
{
    public Guid NozzleId { get; set; }
    public Guid MachineId { get; set; }
    public Guid FuelTypeId { get; set; }

    public string NozzleNumber { get; set; } = string.Empty;
    public string? NozzleName { get; set; }

    /// <summary>
    /// Current cumulative meter reading in liters/kg
    /// This is the total reading shown on the nozzle meter
    /// </summary>
    public decimal CurrentMeterReading { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public Machine? Machine { get; set; }
    public FuelType? FuelType { get; set; }
}
