using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Fuel types available at the petrol pump
/// </summary>
public class FuelType : TenantEntity
{
    public Guid FuelTypeId { get; set; }
    public string FuelName { get; set; } = string.Empty; // Petrol, Diesel, CNG
    public string FuelCode { get; set; } = string.Empty; // PTR, DSL, CNG
    public string Unit { get; set; } = "Liters"; // Liters, Kg
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<FuelRate> FuelRates { get; set; } = new List<FuelRate>();
}
