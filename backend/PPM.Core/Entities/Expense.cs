using PPM.Core.Common;

namespace PPM.Core.Entities;

public class Expense : TenantEntity
{
    public Guid ExpenseId { get; set; }
    public ExpenseCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public PaymentMethod PaymentMode { get; set; }
    public string? Reference { get; set; }  // Bill/Invoice number
    public string? Vendor { get; set; }     // Vendor/Payee name
    public string? Notes { get; set; }
    public Guid RecordedById { get; set; }  // User who recorded this expense

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public User? RecordedBy { get; set; }
}

public enum ExpenseCategory
{
    Electricity = 0,
    Rent = 1,
    Salary = 2,
    Maintenance = 3,
    EquipmentRepair = 4,
    OfficeSupplies = 5,
    Transportation = 6,
    Taxes = 7,
    Insurance = 8,
    Marketing = 9,
    Utilities = 10,      // Water, Gas, etc.
    Communication = 11,  // Phone, Internet
    BankCharges = 12,
    LicenseFees = 13,
    Other = 99
}
