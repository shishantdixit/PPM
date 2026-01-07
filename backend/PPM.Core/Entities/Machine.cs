using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Represents a fuel dispenser machine at the petrol pump
/// </summary>
public class Machine : TenantEntity
{
    public Guid MachineId { get; set; }

    public string MachineName { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public DateOnly? InstallationDate { get; set; }
    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public ICollection<Nozzle> Nozzles { get; set; } = new List<Nozzle>();
}
