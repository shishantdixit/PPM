using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Represents a stock movement entry (In, Out, or Adjustment)
/// </summary>
public class StockEntry : TenantEntity
{
    public Guid StockEntryId { get; set; }
    public Guid TankId { get; set; }
    public StockEntryType EntryType { get; set; }               // In, Out, Adjustment
    public decimal Quantity { get; set; }                       // Positive for In, Negative for Out/Adjustment
    public decimal StockBefore { get; set; }                    // Stock level before this entry
    public decimal StockAfter { get; set; }                     // Stock level after this entry
    public DateTime EntryDate { get; set; }                     // When this entry occurred
    public string? Reference { get; set; }                      // Invoice/Challan number for Stock In
    public string? Vendor { get; set; }                         // Supplier name for Stock In
    public decimal? UnitPrice { get; set; }                     // Price per unit for Stock In
    public decimal? TotalAmount { get; set; }                   // Total cost for Stock In
    public string? Notes { get; set; }
    public Guid? ShiftId { get; set; }                          // If this is an auto-deduction from sales
    public Guid? FuelSaleId { get; set; }                       // If linked to a specific fuel sale
    public Guid RecordedById { get; set; }                      // User who recorded this entry

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public Tank? Tank { get; set; }
    public Shift? Shift { get; set; }
    public FuelSale? FuelSale { get; set; }
    public User? RecordedBy { get; set; }
}

public enum StockEntryType
{
    StockIn = 0,        // Fuel received from supplier
    StockOut = 1,       // Fuel sold (auto-deducted from sales)
    Adjustment = 2,     // Manual adjustment (loss, variance correction)
    Transfer = 3        // Transfer between tanks (future use)
}
