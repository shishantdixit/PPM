using PPM.Core.Common;

namespace PPM.Core.Entities;

public class Shift : TenantEntity
{
    public Guid ShiftId { get; set; }
    public Guid WorkerId { get; set; }
    public Guid MachineId { get; set; }  // Machine the worker is assigned to
    public DateOnly ShiftDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public ShiftStatus Status { get; set; } = ShiftStatus.Pending;

    // Settlement fields
    public decimal TotalSales { get; set; } = 0;
    public decimal CashCollected { get; set; } = 0;
    public decimal CreditSales { get; set; } = 0;
    public decimal DigitalPayments { get; set; } = 0;
    public decimal Borrowing { get; set; } = 0;
    public decimal Variance { get; set; } = 0;
    public string? Notes { get; set; }

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public User? Worker { get; set; }
    public Machine? Machine { get; set; }
    public ICollection<ShiftNozzleReading> NozzleReadings { get; set; } = new List<ShiftNozzleReading>();
    public ICollection<FuelSale> FuelSales { get; set; } = new List<FuelSale>();
}

public enum ShiftStatus
{
    Pending = 0,    // Shift created, not started yet
    Active = 1,     // Shift in progress
    Closed = 2      // Shift completed and settled
}
