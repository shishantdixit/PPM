using PPM.Core.Entities;

namespace PPM.Application.DTOs.Credit;

public class CreditCustomerDto
{
    public Guid CreditCustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AvailableCredit { get; set; }
    public int PaymentTermDays { get; set; }
    public bool IsActive { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockReason { get; set; }
    public string? VehicleNumbers { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateCreditCustomerDto
{
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public decimal CreditLimit { get; set; }
    public int PaymentTermDays { get; set; } = 30;
    public string? VehicleNumbers { get; set; }
    public string? Notes { get; set; }
}

public class UpdateCreditCustomerDto
{
    public string? CustomerName { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? PaymentTermDays { get; set; }
    public string? VehicleNumbers { get; set; }
    public string? Notes { get; set; }
    public bool? IsActive { get; set; }
}

public class CreditTransactionDto
{
    public Guid CreditTransactionId { get; set; }
    public Guid CreditCustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public CreditTransactionType TransactionType { get; set; }
    public string TransactionTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime TransactionDate { get; set; }
    public Guid? FuelSaleId { get; set; }
    public string? SaleNumber { get; set; }
    public PaymentMode? PaymentMode { get; set; }
    public string? PaymentModeName { get; set; }
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecordPaymentDto
{
    public decimal Amount { get; set; }
    public PaymentMode PaymentMode { get; set; }
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
}

public class AdjustBalanceDto
{
    public decimal Amount { get; set; } // Positive increases balance, negative decreases
    public string Notes { get; set; } = string.Empty;
}

public class BlockCustomerDto
{
    public string Reason { get; set; } = string.Empty;
}

public class CreditCustomerSummaryDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int BlockedCustomers { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal TotalCreditLimit { get; set; }
    public List<CreditCustomerDto> TopDebtors { get; set; } = new();
    public List<CreditCustomerDto> OverdueCustomers { get; set; } = new();
}

public class CustomerStatementDto
{
    public CreditCustomerDto Customer { get; set; } = null!;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<CreditTransactionDto> Transactions { get; set; } = new();
}
