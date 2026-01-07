namespace PPM.Application.DTOs.Fuel;

public class FuelTypeDto
{
    public Guid FuelTypeId { get; set; }
    public Guid TenantId { get; set; }
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateFuelTypeDto
{
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public string Unit { get; set; } = "Liters";
}

public class UpdateFuelTypeDto
{
    public string? FuelName { get; set; }
    public string? Unit { get; set; }
    public bool? IsActive { get; set; }
}
