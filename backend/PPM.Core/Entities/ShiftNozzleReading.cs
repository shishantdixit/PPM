namespace PPM.Core.Entities;

public class ShiftNozzleReading : TenantEntity
{
    public Guid ShiftNozzleReadingId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid NozzleId { get; set; }

    public decimal OpeningReading { get; set; }
    public decimal? ClosingReading { get; set; }
    public decimal QuantitySold { get; set; } = 0;
    public decimal RateAtShift { get; set; }  // Fuel rate at the time of shift
    public decimal ExpectedAmount { get; set; } = 0;

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public Shift? Shift { get; set; }
    public Nozzle? Nozzle { get; set; }
}
