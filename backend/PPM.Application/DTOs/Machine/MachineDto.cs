namespace PPM.Application.DTOs.Machine;

public class MachineDto
{
    public Guid MachineId { get; set; }
    public Guid TenantId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public DateOnly? InstallationDate { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public int NozzleCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateMachineDto
{
    public string MachineName { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public DateOnly? InstallationDate { get; set; }
    public string? Location { get; set; }
}

public class UpdateMachineDto
{
    public string? MachineName { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public DateOnly? InstallationDate { get; set; }
    public string? Location { get; set; }
    public bool? IsActive { get; set; }
}
