using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Represents a customer who has a credit account at the petrol pump
/// </summary>
public class CreditCustomer : TenantEntity
{
    public Guid CreditCustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }

    // Credit account details
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; } // Outstanding amount
    public int PaymentTermDays { get; set; } = 30; // Default payment terms

    // Status
    public bool IsActive { get; set; } = true;
    public bool IsBlocked { get; set; } // Block new credit if limit exceeded or payment overdue
    public string? BlockReason { get; set; }

    // Vehicle registration numbers (comma-separated for easy matching)
    public string? VehicleNumbers { get; set; }

    // Notes
    public string? Notes { get; set; }

    // Navigation
    public Tenant? Tenant { get; set; }
    public ICollection<CreditTransaction> Transactions { get; set; } = new List<CreditTransaction>();
}

/// <summary>
/// Represents a credit transaction (sale on credit or payment received)
/// </summary>
public class CreditTransaction : TenantEntity
{
    public Guid CreditTransactionId { get; set; }
    public Guid CreditCustomerId { get; set; }

    // Transaction details
    public CreditTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; } // Customer balance after this transaction
    public DateTime TransactionDate { get; set; }

    // Reference to sale (if type is Sale)
    public Guid? FuelSaleId { get; set; }

    // Payment details (if type is Payment)
    public PaymentMode? PaymentMode { get; set; }
    public string? PaymentReference { get; set; } // Cheque number, transaction ID, etc.

    // Notes
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }

    // Navigation
    public CreditCustomer? CreditCustomer { get; set; }
    public FuelSale? FuelSale { get; set; }
    public Tenant? Tenant { get; set; }
}

public enum CreditTransactionType
{
    Sale = 0,       // Credit sale - increases balance
    Payment = 1,    // Payment received - decreases balance
    Adjustment = 2, // Manual adjustment (positive or negative)
    Refund = 3      // Refund - decreases balance
}

public enum PaymentMode
{
    Cash = 0,
    Cheque = 1,
    BankTransfer = 2,
    UPI = 3,
    Card = 4,
    Other = 5
}
