namespace PPM.Application.DTOs.Fuel;

public class FuelRateDto
{
    public Guid FuelRateId { get; set; }
    public Guid TenantId { get; set; }
    public Guid FuelTypeId { get; set; }
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsCurrent => EffectiveTo == null;
}

public class CreateFuelRateDto
{
    public Guid FuelTypeId { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveFrom { get; set; }
}

public class UpdateFuelRateDto
{
    public decimal Rate { get; set; }
    public DateTime EffectiveFrom { get; set; }
}

public class CurrentFuelRateDto
{
    public Guid FuelTypeId { get; set; }
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal CurrentRate { get; set; }
    public DateTime EffectiveFrom { get; set; }
}
