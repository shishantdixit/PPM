namespace PPM.Application.DTOs.Nozzle;

public class NozzleDto
{
    public Guid NozzleId { get; set; }
    public Guid TenantId { get; set; }
    public Guid MachineId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public Guid FuelTypeId { get; set; }
    public string FuelName { get; set; } = string.Empty;
    public string FuelCode { get; set; } = string.Empty;
    public string FuelUnit { get; set; } = string.Empty;
    public string NozzleNumber { get; set; } = string.Empty;
    public string? NozzleName { get; set; }
    public decimal CurrentMeterReading { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateNozzleDto
{
    public Guid MachineId { get; set; }
    public Guid FuelTypeId { get; set; }
    public string NozzleNumber { get; set; } = string.Empty;
    public string? NozzleName { get; set; }
    public decimal CurrentMeterReading { get; set; } = 0;
}

public class UpdateNozzleDto
{
    public string? NozzleName { get; set; }
    public decimal? CurrentMeterReading { get; set; }
    public bool? IsActive { get; set; }
}
