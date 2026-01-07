using PPM.Core.Entities;

namespace PPM.Application.DTOs.Expense;

public class ExpenseDto
{
    public Guid ExpenseId { get; set; }
    public Guid TenantId { get; set; }
    public ExpenseCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public PaymentMethod PaymentMode { get; set; }
    public string PaymentModeName { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Vendor { get; set; }
    public string? Notes { get; set; }
    public Guid RecordedById { get; set; }
    public string RecordedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateExpenseDto
{
    public ExpenseCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public PaymentMethod PaymentMode { get; set; }
    public string? Reference { get; set; }
    public string? Vendor { get; set; }
    public string? Notes { get; set; }
}

public class UpdateExpenseDto
{
    public ExpenseCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public PaymentMethod PaymentMode { get; set; }
    public string? Reference { get; set; }
    public string? Vendor { get; set; }
    public string? Notes { get; set; }
}

public class ExpenseSummaryDto
{
    public decimal TotalExpenses { get; set; }
    public List<CategorySummaryDto> ByCategory { get; set; } = new();
    public List<DailySummaryDto> ByDate { get; set; } = new();
}

public class CategorySummaryDto
{
    public ExpenseCategory Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Count { get; set; }
}

public class DailySummaryDto
{
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}
