namespace PPM.Core.Entities;

public class FuelSale : TenantEntity
{
    public Guid FuelSaleId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid NozzleId { get; set; }

    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? VehicleNumber { get; set; }
    public DateTime SaleTime { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public Shift? Shift { get; set; }
    public Nozzle? Nozzle { get; set; }
}

public enum PaymentMethod
{
    Cash = 0,
    Credit = 1,
    Digital = 2,      // UPI, Card, etc.
    Mixed = 3         // Combination of payment methods
}
