using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Historical fuel rate tracking
/// </summary>
public class FuelRate : TenantEntity
{
    public Guid FuelRateId { get; set; }
    public Guid FuelTypeId { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; } // NULL means current rate
    public Guid? UpdatedBy { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual FuelType FuelType { get; set; } = null!;
    public virtual User? UpdatedByUser { get; set; }
}
