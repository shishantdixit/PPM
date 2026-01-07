using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.API.Services;
using PPM.Application.Common;
using PPM.Application.DTOs.Expense;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class ExpensesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ExpensesController> _logger;
    private readonly IExportService _exportService;

    public ExpensesController(ApplicationDbContext dbContext, ILogger<ExpensesController> logger, IExportService exportService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _exportService = exportService;
    }

    /// <summary>
    /// Get expense summary/dashboard
    /// </summary>
    [HttpGet("summary")]
    [RequireManager]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<ExpenseSummaryDto>.ErrorResponse("Tenant context not found"));

            var to = toDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? to.AddDays(-30);

            var expenses = await _dbContext.Expenses
                .Where(e => e.TenantId == tenantId.Value &&
                           e.ExpenseDate >= from &&
                           e.ExpenseDate <= to)
                .ToListAsync();

            var summary = new ExpenseSummaryDto
            {
                TotalExpenses = expenses.Sum(e => e.Amount),
                ByCategory = expenses
                    .GroupBy(e => e.Category)
                    .Select(g => new CategorySummaryDto
                    {
                        Category = g.Key,
                        CategoryName = g.Key.ToString(),
                        Amount = g.Sum(e => e.Amount),
                        Count = g.Count()
                    })
                    .OrderByDescending(c => c.Amount)
                    .ToList(),
                ByDate = expenses
                    .GroupBy(e => e.ExpenseDate)
                    .Select(g => new DailySummaryDto
                    {
                        Date = g.Key,
                        Amount = g.Sum(e => e.Amount),
                        Count = g.Count()
                    })
                    .OrderBy(d => d.Date)
                    .ToList()
            };

            return Ok(ApiResponse<ExpenseSummaryDto>.SuccessResponse(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching expense summary");
            return StatusCode(500, ApiResponse<ExpenseSummaryDto>.ErrorResponse("Failed to fetch summary"));
        }
    }

    /// <summary>
    /// Get all expenses with optional filtering
    /// </summary>
    [HttpGet]
    [RequireManager]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] ExpenseCategory? category = null,
        [FromQuery] PaymentMethod? paymentMode = null,
        [FromQuery] string? search = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<ExpenseDto>>.ErrorResponse("Tenant context not found"));

            var query = _dbContext.Expenses
                .Include(e => e.RecordedBy)
                .Where(e => e.TenantId == tenantId.Value);

            if (fromDate.HasValue)
                query = query.Where(e => e.ExpenseDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.ExpenseDate <= toDate.Value);

            if (category.HasValue)
                query = query.Where(e => e.Category == category.Value);

            if (paymentMode.HasValue)
                query = query.Where(e => e.PaymentMode == paymentMode.Value);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(e =>
                    e.Description.ToLower().Contains(search) ||
                    (e.Vendor != null && e.Vendor.ToLower().Contains(search)) ||
                    (e.Reference != null && e.Reference.ToLower().Contains(search)));
            }

            var expenses = await query
                .OrderByDescending(e => e.ExpenseDate)
                .ThenByDescending(e => e.CreatedAt)
                .Select(e => MapToDto(e))
                .ToListAsync();

            return Ok(ApiResponse<List<ExpenseDto>>.SuccessResponse(expenses));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching expenses");
            return StatusCode(500, ApiResponse<List<ExpenseDto>>.ErrorResponse("Failed to fetch expenses"));
        }
    }

    /// <summary>
    /// Get expense by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequireManager]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<ExpenseDto>.ErrorResponse("Tenant context not found"));

            var expense = await _dbContext.Expenses
                .Include(e => e.RecordedBy)
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.TenantId == tenantId.Value);

            if (expense == null)
                return NotFound(ApiResponse<ExpenseDto>.ErrorResponse("Expense not found"));

            return Ok(ApiResponse<ExpenseDto>.SuccessResponse(MapToDto(expense)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching expense {ExpenseId}", id);
            return StatusCode(500, ApiResponse<ExpenseDto>.ErrorResponse("Failed to fetch expense"));
        }
    }

    /// <summary>
    /// Create a new expense
    /// </summary>
    [HttpPost]
    [RequireManager]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var userId = HttpContext.GetUserId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<ExpenseDto>.ErrorResponse("Tenant context not found"));

            if (dto.Amount <= 0)
                return BadRequest(ApiResponse<ExpenseDto>.ErrorResponse("Amount must be greater than zero"));

            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest(ApiResponse<ExpenseDto>.ErrorResponse("Description is required"));

            var expense = new Expense
            {
                ExpenseId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                Category = dto.Category,
                Description = dto.Description,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                PaymentMode = dto.PaymentMode,
                Reference = dto.Reference,
                Vendor = dto.Vendor,
                Notes = dto.Notes,
                RecordedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Expenses.Add(expense);
            await _dbContext.SaveChangesAsync();

            // Reload with navigation properties
            await _dbContext.Entry(expense).Reference(e => e.RecordedBy).LoadAsync();

            _logger.LogInformation("Created expense {ExpenseId} for {Amount}", expense.ExpenseId, expense.Amount);

            return CreatedAtAction(nameof(GetById), new { id = expense.ExpenseId },
                ApiResponse<ExpenseDto>.SuccessResponse(MapToDto(expense), "Expense created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense");
            return StatusCode(500, ApiResponse<ExpenseDto>.ErrorResponse("Failed to create expense"));
        }
    }

    /// <summary>
    /// Update an existing expense
    /// </summary>
    [HttpPut("{id}")]
    [RequireManager]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<ExpenseDto>.ErrorResponse("Tenant context not found"));

            var expense = await _dbContext.Expenses
                .Include(e => e.RecordedBy)
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.TenantId == tenantId.Value);

            if (expense == null)
                return NotFound(ApiResponse<ExpenseDto>.ErrorResponse("Expense not found"));

            if (dto.Amount <= 0)
                return BadRequest(ApiResponse<ExpenseDto>.ErrorResponse("Amount must be greater than zero"));

            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest(ApiResponse<ExpenseDto>.ErrorResponse("Description is required"));

            expense.Category = dto.Category;
            expense.Description = dto.Description;
            expense.Amount = dto.Amount;
            expense.ExpenseDate = dto.ExpenseDate;
            expense.PaymentMode = dto.PaymentMode;
            expense.Reference = dto.Reference;
            expense.Vendor = dto.Vendor;
            expense.Notes = dto.Notes;
            expense.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated expense {ExpenseId}", id);

            return Ok(ApiResponse<ExpenseDto>.SuccessResponse(MapToDto(expense), "Expense updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating expense {ExpenseId}", id);
            return StatusCode(500, ApiResponse<ExpenseDto>.ErrorResponse("Failed to update expense"));
        }
    }

    /// <summary>
    /// Delete an expense
    /// </summary>
    [HttpDelete("{id}")]
    [RequireOwner]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var expense = await _dbContext.Expenses
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.TenantId == tenantId.Value);

            if (expense == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Expense not found"));

            _dbContext.Expenses.Remove(expense);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted expense {ExpenseId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Expense deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expense {ExpenseId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete expense"));
        }
    }

    /// <summary>
    /// Get expense category options
    /// </summary>
    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var categories = Enum.GetValues<ExpenseCategory>()
            .Select(c => new { Value = (int)c, Name = c.ToString() })
            .ToList();

        return Ok(ApiResponse<object>.SuccessResponse(categories));
    }

    /// <summary>
    /// Export expenses to PDF/Excel/CSV
    /// </summary>
    [HttpGet("export")]
    [RequireManager]
    public async Task<IActionResult> Export(
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] ExpenseCategory? category = null,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var to = toDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? to.AddDays(-30);

            var query = _dbContext.Expenses
                .Include(e => e.RecordedBy)
                .Where(e => e.TenantId == tenantId.Value &&
                           e.ExpenseDate >= from &&
                           e.ExpenseDate <= to);

            if (category.HasValue)
                query = query.Where(e => e.Category == category.Value);

            var expenses = await query.OrderByDescending(e => e.ExpenseDate).ToListAsync();

            var headers = new[] { "Date", "Category", "Description", "Vendor", "Amount", "Payment Mode", "Reference", "Recorded By" };
            var rows = expenses.Select(e => new[]
            {
                e.ExpenseDate.ToString("dd-MMM-yyyy"),
                e.Category.ToString(),
                e.Description,
                e.Vendor ?? "-",
                e.Amount.ToString("N2"),
                e.PaymentMode.ToString(),
                e.Reference ?? "-",
                e.RecordedBy?.FullName ?? "-"
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Records", expenses.Count.ToString() },
                { "Total Amount", expenses.Sum(e => e.Amount).ToString("N2") }
            };

            if (category.HasValue)
                summary.Add("Category Filter", category.Value.ToString());

            var title = "Expense Report";
            return GenerateExport(title, headers, rows, summary, format, "expenses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting expenses");
            return StatusCode(500, "Failed to export expenses");
        }
    }

    /// <summary>
    /// Export expense summary by category
    /// </summary>
    [HttpGet("summary/export")]
    [RequireManager]
    public async Task<IActionResult> ExportSummary(
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var to = toDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? to.AddDays(-30);

            var expenses = await _dbContext.Expenses
                .Where(e => e.TenantId == tenantId.Value &&
                           e.ExpenseDate >= from &&
                           e.ExpenseDate <= to)
                .ToListAsync();

            var categoryData = expenses
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key.ToString(),
                    Count = g.Count(),
                    Amount = g.Sum(e => e.Amount),
                    Percentage = expenses.Sum(e => e.Amount) > 0
                        ? (g.Sum(e => e.Amount) / expenses.Sum(e => e.Amount) * 100).ToString("N1") + "%"
                        : "0%"
                })
                .OrderByDescending(c => c.Amount)
                .ToList();

            var headers = new[] { "Category", "Count", "Amount", "% of Total" };
            var rows = categoryData.Select(c => new[]
            {
                c.Category,
                c.Count.ToString(),
                c.Amount.ToString("N2"),
                c.Percentage
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Categories", categoryData.Count.ToString() },
                { "Total Expenses", expenses.Count.ToString() },
                { "Total Amount", expenses.Sum(e => e.Amount).ToString("N2") }
            };

            var title = "Expense Summary by Category";
            return GenerateExport(title, headers, rows, summary, format, "expense-summary");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting expense summary");
            return StatusCode(500, "Failed to export expense summary");
        }
    }

    private IActionResult GenerateExport(string title, string[] headers, List<string[]> rows, Dictionary<string, string> summary, string format, string fileName)
    {
        byte[] fileBytes;
        string contentType;
        string extension;

        switch (format.ToLower())
        {
            case "excel":
            case "xlsx":
                fileBytes = _exportService.GenerateExcel(title, headers, rows, summary);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                extension = "xlsx";
                break;

            case "csv":
                fileBytes = _exportService.GenerateCsv(headers, rows);
                contentType = "text/csv";
                extension = "csv";
                break;

            default: // pdf
                fileBytes = _exportService.GeneratePdf(title, headers, rows, summary);
                contentType = "application/pdf";
                extension = "pdf";
                break;
        }

        return File(fileBytes, contentType, $"{fileName}-{DateTime.Now:yyyyMMdd}.{extension}");
    }

    private static ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            ExpenseId = expense.ExpenseId,
            TenantId = expense.TenantId,
            Category = expense.Category,
            CategoryName = expense.Category.ToString(),
            Description = expense.Description,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            PaymentMode = expense.PaymentMode,
            PaymentModeName = expense.PaymentMode.ToString(),
            Reference = expense.Reference,
            Vendor = expense.Vendor,
            Notes = expense.Notes,
            RecordedById = expense.RecordedById,
            RecordedByName = expense.RecordedBy?.FullName ?? "Unknown",
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
