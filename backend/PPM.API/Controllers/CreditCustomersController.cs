using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.API.Services;
using PPM.Application.Common;
using PPM.Application.DTOs.Credit;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class CreditCustomersController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CreditCustomersController> _logger;
    private readonly IExportService _exportService;

    public CreditCustomersController(ApplicationDbContext dbContext, ILogger<CreditCustomersController> logger, IExportService exportService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _exportService = exportService;
    }

    /// <summary>
    /// Get credit customer summary/dashboard
    /// </summary>
    [HttpGet("summary")]
    [RequireManager]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditCustomerSummaryDto>.ErrorResponse("Tenant context not found"));

            var customers = await _dbContext.CreditCustomers
                .Where(c => c.TenantId == tenantId.Value)
                .ToListAsync();

            var summary = new CreditCustomerSummaryDto
            {
                TotalCustomers = customers.Count,
                ActiveCustomers = customers.Count(c => c.IsActive && !c.IsBlocked),
                BlockedCustomers = customers.Count(c => c.IsBlocked),
                TotalOutstanding = customers.Sum(c => c.CurrentBalance),
                TotalCreditLimit = customers.Sum(c => c.CreditLimit),
                TopDebtors = customers
                    .Where(c => c.CurrentBalance > 0)
                    .OrderByDescending(c => c.CurrentBalance)
                    .Take(5)
                    .Select(c => MapToDto(c))
                    .ToList(),
                OverdueCustomers = new List<CreditCustomerDto>() // Would need payment term tracking
            };

            return Ok(ApiResponse<CreditCustomerSummaryDto>.SuccessResponse(summary));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching credit customer summary");
            return StatusCode(500, ApiResponse<CreditCustomerSummaryDto>.ErrorResponse("Failed to fetch summary"));
        }
    }

    /// <summary>
    /// Get all credit customers
    /// </summary>
    [HttpGet]
    [RequireManager]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isBlocked = null,
        [FromQuery] string? search = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<CreditCustomerDto>>.ErrorResponse("Tenant context not found"));

            var query = _dbContext.CreditCustomers.Where(c => c.TenantId == tenantId.Value);

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (isBlocked.HasValue)
                query = query.Where(c => c.IsBlocked == isBlocked.Value);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(c =>
                    c.CustomerName.ToLower().Contains(search) ||
                    c.CustomerCode.ToLower().Contains(search) ||
                    c.Phone.Contains(search) ||
                    (c.VehicleNumbers != null && c.VehicleNumbers.ToLower().Contains(search)));
            }

            var customers = await query
                .OrderBy(c => c.CustomerName)
                .Select(c => MapToDto(c))
                .ToListAsync();

            return Ok(ApiResponse<List<CreditCustomerDto>>.SuccessResponse(customers));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching credit customers");
            return StatusCode(500, ApiResponse<List<CreditCustomerDto>>.ErrorResponse("Failed to fetch customers"));
        }
    }

    /// <summary>
    /// Get credit customer by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequireManager]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditCustomerDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<CreditCustomerDto>.ErrorResponse("Customer not found"));

            return Ok(ApiResponse<CreditCustomerDto>.SuccessResponse(MapToDto(customer)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching credit customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CreditCustomerDto>.ErrorResponse("Failed to fetch customer"));
        }
    }

    /// <summary>
    /// Create a new credit customer
    /// </summary>
    [HttpPost]
    [RequireManager]
    public async Task<IActionResult> Create([FromBody] CreateCreditCustomerDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditCustomerDto>.ErrorResponse("Tenant context not found"));

            // Check if customer code already exists
            var existingCustomer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CustomerCode == dto.CustomerCode && c.TenantId == tenantId.Value);

            if (existingCustomer != null)
                return BadRequest(ApiResponse<CreditCustomerDto>.ErrorResponse("Customer code already exists"));

            var customer = new CreditCustomer
            {
                CreditCustomerId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                CustomerCode = dto.CustomerCode,
                CustomerName = dto.CustomerName,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                CreditLimit = dto.CreditLimit,
                CurrentBalance = 0,
                PaymentTermDays = dto.PaymentTermDays,
                VehicleNumbers = dto.VehicleNumbers,
                Notes = dto.Notes,
                IsActive = true,
                IsBlocked = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.CreditCustomers.Add(customer);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created credit customer {CustomerCode}", customer.CustomerCode);

            return CreatedAtAction(nameof(GetById), new { id = customer.CreditCustomerId },
                ApiResponse<CreditCustomerDto>.SuccessResponse(MapToDto(customer), "Customer created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating credit customer");
            return StatusCode(500, ApiResponse<CreditCustomerDto>.ErrorResponse("Failed to create customer"));
        }
    }

    /// <summary>
    /// Update credit customer details
    /// </summary>
    [HttpPut("{id}")]
    [RequireManager]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCreditCustomerDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditCustomerDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<CreditCustomerDto>.ErrorResponse("Customer not found"));

            // Update fields if provided
            if (dto.CustomerName != null) customer.CustomerName = dto.CustomerName;
            if (dto.ContactPerson != null) customer.ContactPerson = dto.ContactPerson;
            if (dto.Phone != null) customer.Phone = dto.Phone;
            if (dto.Email != null) customer.Email = dto.Email;
            if (dto.Address != null) customer.Address = dto.Address;
            if (dto.CreditLimit.HasValue) customer.CreditLimit = dto.CreditLimit.Value;
            if (dto.PaymentTermDays.HasValue) customer.PaymentTermDays = dto.PaymentTermDays.Value;
            if (dto.VehicleNumbers != null) customer.VehicleNumbers = dto.VehicleNumbers;
            if (dto.Notes != null) customer.Notes = dto.Notes;
            if (dto.IsActive.HasValue) customer.IsActive = dto.IsActive.Value;

            customer.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated credit customer {CustomerId}", id);

            return Ok(ApiResponse<CreditCustomerDto>.SuccessResponse(MapToDto(customer), "Customer updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating credit customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CreditCustomerDto>.ErrorResponse("Failed to update customer"));
        }
    }

    /// <summary>
    /// Record a payment from credit customer
    /// </summary>
    [HttpPost("{id}/payments")]
    [RequireManager]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordPaymentDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var userId = HttpContext.GetUserId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditTransactionDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<CreditTransactionDto>.ErrorResponse("Customer not found"));

            if (dto.Amount <= 0)
                return BadRequest(ApiResponse<CreditTransactionDto>.ErrorResponse("Payment amount must be positive"));

            // Get user name for tracking
            var userName = "System";
            if (userId != Guid.Empty)
            {
                var user = await _dbContext.Users.FindAsync(userId);
                userName = user?.FullName ?? "Unknown";
            }

            // Update customer balance
            customer.CurrentBalance -= dto.Amount;
            customer.UpdatedAt = DateTime.UtcNow;

            // If was blocked due to over limit and now under limit, can optionally unblock
            if (customer.IsBlocked && customer.CurrentBalance <= customer.CreditLimit)
            {
                // Keep blocked - manual unblock required
            }

            // Create transaction record
            var transaction = new CreditTransaction
            {
                CreditTransactionId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                CreditCustomerId = customer.CreditCustomerId,
                TransactionType = CreditTransactionType.Payment,
                Amount = dto.Amount,
                BalanceAfter = customer.CurrentBalance,
                TransactionDate = DateTime.UtcNow,
                PaymentMode = dto.PaymentMode,
                PaymentReference = dto.PaymentReference,
                Notes = dto.Notes,
                CreatedBy = userName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.CreditTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Recorded payment of {Amount} for customer {CustomerId}", dto.Amount, id);

            var result = await MapTransactionToDto(transaction);
            return Ok(ApiResponse<CreditTransactionDto>.SuccessResponse(result, "Payment recorded successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment for customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CreditTransactionDto>.ErrorResponse("Failed to record payment"));
        }
    }

    /// <summary>
    /// Adjust customer balance (manual correction)
    /// </summary>
    [HttpPost("{id}/adjustments")]
    [RequireOwner]
    public async Task<IActionResult> AdjustBalance(Guid id, [FromBody] AdjustBalanceDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            var userId = HttpContext.GetUserId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditTransactionDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<CreditTransactionDto>.ErrorResponse("Customer not found"));

            if (string.IsNullOrWhiteSpace(dto.Notes))
                return BadRequest(ApiResponse<CreditTransactionDto>.ErrorResponse("Adjustment reason is required"));

            // Get user name
            var userName = "System";
            if (userId != Guid.Empty)
            {
                var user = await _dbContext.Users.FindAsync(userId);
                userName = user?.FullName ?? "Unknown";
            }

            // Update balance
            customer.CurrentBalance += dto.Amount;
            customer.UpdatedAt = DateTime.UtcNow;

            // Create adjustment transaction
            var transaction = new CreditTransaction
            {
                CreditTransactionId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                CreditCustomerId = customer.CreditCustomerId,
                TransactionType = CreditTransactionType.Adjustment,
                Amount = Math.Abs(dto.Amount),
                BalanceAfter = customer.CurrentBalance,
                TransactionDate = DateTime.UtcNow,
                Notes = $"Adjustment ({(dto.Amount >= 0 ? "+" : "-")}): {dto.Notes}",
                CreatedBy = userName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.CreditTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Adjusted balance by {Amount} for customer {CustomerId}", dto.Amount, id);

            var result = await MapTransactionToDto(transaction);
            return Ok(ApiResponse<CreditTransactionDto>.SuccessResponse(result, "Balance adjusted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adjusting balance for customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CreditTransactionDto>.ErrorResponse("Failed to adjust balance"));
        }
    }

    /// <summary>
    /// Block a credit customer
    /// </summary>
    [HttpPost("{id}/block")]
    [RequireManager]
    public async Task<IActionResult> BlockCustomer(Guid id, [FromBody] BlockCustomerDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditCustomerDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<CreditCustomerDto>.ErrorResponse("Customer not found"));

            customer.IsBlocked = true;
            customer.BlockReason = dto.Reason;
            customer.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Blocked credit customer {CustomerId}: {Reason}", id, dto.Reason);

            return Ok(ApiResponse<CreditCustomerDto>.SuccessResponse(MapToDto(customer), "Customer blocked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blocking customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CreditCustomerDto>.ErrorResponse("Failed to block customer"));
        }
    }

    /// <summary>
    /// Unblock a credit customer
    /// </summary>
    [HttpPost("{id}/unblock")]
    [RequireManager]
    public async Task<IActionResult> UnblockCustomer(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditCustomerDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<CreditCustomerDto>.ErrorResponse("Customer not found"));

            customer.IsBlocked = false;
            customer.BlockReason = null;
            customer.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Unblocked credit customer {CustomerId}", id);

            return Ok(ApiResponse<CreditCustomerDto>.SuccessResponse(MapToDto(customer), "Customer unblocked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unblocking customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CreditCustomerDto>.ErrorResponse("Failed to unblock customer"));
        }
    }

    /// <summary>
    /// Get transaction history for a customer
    /// </summary>
    [HttpGet("{id}/transactions")]
    [RequireManager]
    public async Task<IActionResult> GetTransactions(
        Guid id,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] CreditTransactionType? type = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<List<CreditTransactionDto>>.ErrorResponse("Tenant context not found"));

            // Verify customer exists and belongs to tenant
            var customerExists = await _dbContext.CreditCustomers
                .AnyAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (!customerExists)
                return NotFound(ApiResponse<List<CreditTransactionDto>>.ErrorResponse("Customer not found"));

            var query = _dbContext.CreditTransactions
                .Include(t => t.FuelSale)
                .Where(t => t.CreditCustomerId == id && t.TenantId == tenantId.Value);

            if (fromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.TransactionDate <= toDate.Value.AddDays(1));

            if (type.HasValue)
                query = query.Where(t => t.TransactionType == type.Value);

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            var results = new List<CreditTransactionDto>();
            foreach (var t in transactions)
            {
                results.Add(await MapTransactionToDto(t));
            }

            return Ok(ApiResponse<List<CreditTransactionDto>>.SuccessResponse(results));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching transactions for customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<List<CreditTransactionDto>>.ErrorResponse("Failed to fetch transactions"));
        }
    }

    /// <summary>
    /// Get customer statement (account summary with transactions)
    /// </summary>
    [HttpGet("{id}/statement")]
    [RequireManager]
    public async Task<IActionResult> GetStatement(
        Guid id,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CustomerStatementDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<CustomerStatementDto>.ErrorResponse("Customer not found"));

            // Default date range: last 30 days
            var to = toDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? to.AddDays(-30);

            var fromDateTime = from.ToDateTime(TimeOnly.MinValue);
            var toDateTime = to.ToDateTime(TimeOnly.MaxValue);

            // Get transactions in range
            var transactions = await _dbContext.CreditTransactions
                .Include(t => t.FuelSale)
                .Where(t => t.CreditCustomerId == id &&
                           t.TenantId == tenantId.Value &&
                           t.TransactionDate >= fromDateTime &&
                           t.TransactionDate <= toDateTime)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            // Calculate opening balance (balance before from date)
            var transactionsBefore = await _dbContext.CreditTransactions
                .Where(t => t.CreditCustomerId == id &&
                           t.TenantId == tenantId.Value &&
                           t.TransactionDate < fromDateTime)
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefaultAsync();

            var openingBalance = transactionsBefore?.BalanceAfter ?? 0;

            // Calculate totals
            var totalSales = transactions
                .Where(t => t.TransactionType == CreditTransactionType.Sale)
                .Sum(t => t.Amount);
            var totalPayments = transactions
                .Where(t => t.TransactionType == CreditTransactionType.Payment)
                .Sum(t => t.Amount);
            var closingBalance = transactions.Any()
                ? transactions.Last().BalanceAfter
                : openingBalance;

            var transactionDtos = new List<CreditTransactionDto>();
            foreach (var t in transactions)
            {
                transactionDtos.Add(await MapTransactionToDto(t));
            }

            var statement = new CustomerStatementDto
            {
                Customer = MapToDto(customer),
                FromDate = from,
                ToDate = to,
                OpeningBalance = openingBalance,
                TotalSales = totalSales,
                TotalPayments = totalPayments,
                ClosingBalance = closingBalance,
                Transactions = transactionDtos
            };

            return Ok(ApiResponse<CustomerStatementDto>.SuccessResponse(statement));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating statement for customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<CustomerStatementDto>.ErrorResponse("Failed to generate statement"));
        }
    }

    /// <summary>
    /// Delete credit customer (only if no transactions)
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

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Customer not found"));

            // Check for existing transactions
            var hasTransactions = await _dbContext.CreditTransactions
                .AnyAsync(t => t.CreditCustomerId == id);

            if (hasTransactions)
            {
                // Soft delete - deactivate
                customer.IsActive = false;
                customer.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Deactivated credit customer {CustomerId} (has transactions)", id);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Customer deactivated (has transaction history)"));
            }

            // Hard delete
            _dbContext.CreditCustomers.Remove(customer);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted credit customer {CustomerId}", id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Customer deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete customer"));
        }
    }

    /// <summary>
    /// Search customers by vehicle number
    /// </summary>
    [HttpGet("by-vehicle/{vehicleNumber}")]
    public async Task<IActionResult> GetByVehicle(string vehicleNumber)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<CreditCustomerDto>.ErrorResponse("Tenant context not found"));

            var customer = await _dbContext.CreditCustomers
                .Where(c => c.TenantId == tenantId.Value &&
                           c.IsActive &&
                           !c.IsBlocked &&
                           c.VehicleNumbers != null &&
                           c.VehicleNumbers.ToLower().Contains(vehicleNumber.ToLower()))
                .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound(ApiResponse<CreditCustomerDto>.ErrorResponse("No credit customer found for this vehicle"));

            return Ok(ApiResponse<CreditCustomerDto>.SuccessResponse(MapToDto(customer)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching customer by vehicle {VehicleNumber}", vehicleNumber);
            return StatusCode(500, ApiResponse<CreditCustomerDto>.ErrorResponse("Failed to search customer"));
        }
    }

    /// <summary>
    /// Export customer statement to PDF/Excel/CSV
    /// </summary>
    [HttpGet("{id}/statement/export")]
    [RequireManager]
    public async Task<IActionResult> ExportStatement(
        Guid id,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var customer = await _dbContext.CreditCustomers
                .FirstOrDefaultAsync(c => c.CreditCustomerId == id && c.TenantId == tenantId.Value);

            if (customer == null)
                return NotFound("Customer not found");

            var to = toDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var from = fromDate ?? to.AddDays(-30);

            var fromDateTime = from.ToDateTime(TimeOnly.MinValue);
            var toDateTime = to.ToDateTime(TimeOnly.MaxValue);

            var transactions = await _dbContext.CreditTransactions
                .Include(t => t.FuelSale)
                .Where(t => t.CreditCustomerId == id &&
                           t.TenantId == tenantId.Value &&
                           t.TransactionDate >= fromDateTime &&
                           t.TransactionDate <= toDateTime)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            var headers = new[] { "Date", "Type", "Reference", "Debit", "Credit", "Balance", "Notes" };
            var rows = transactions.Select(t => new[]
            {
                t.TransactionDate.ToString("dd-MMM-yyyy HH:mm"),
                t.TransactionType.ToString(),
                t.FuelSale?.SaleNumber ?? t.PaymentReference ?? "-",
                t.TransactionType == CreditTransactionType.Sale ? t.Amount.ToString("N2") : "-",
                t.TransactionType != CreditTransactionType.Sale ? t.Amount.ToString("N2") : "-",
                t.BalanceAfter.ToString("N2"),
                t.Notes ?? "-"
            }).ToList();

            var totalSales = transactions.Where(t => t.TransactionType == CreditTransactionType.Sale).Sum(t => t.Amount);
            var totalPayments = transactions.Where(t => t.TransactionType == CreditTransactionType.Payment).Sum(t => t.Amount);

            var summary = new Dictionary<string, string>
            {
                { "Customer", $"{customer.CustomerName} ({customer.CustomerCode})" },
                { "Period", $"{from:dd-MMM-yyyy} to {to:dd-MMM-yyyy}" },
                { "Total Purchases", totalSales.ToString("N2") },
                { "Total Payments", totalPayments.ToString("N2") },
                { "Current Balance", customer.CurrentBalance.ToString("N2") }
            };

            var title = $"Account Statement - {customer.CustomerName}";
            return GenerateExport(title, headers, rows, summary, format, $"statement-{customer.CustomerCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting customer statement {CustomerId}", id);
            return StatusCode(500, "Failed to export statement");
        }
    }

    /// <summary>
    /// Export all credit customers list
    /// </summary>
    [HttpGet("export")]
    [RequireManager]
    public async Task<IActionResult> ExportCustomers(
        [FromQuery] bool? isActive,
        [FromQuery] bool? isBlocked,
        [FromQuery] string format = "pdf")
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest("Tenant context not found");

            var query = _dbContext.CreditCustomers.Where(c => c.TenantId == tenantId.Value);

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (isBlocked.HasValue)
                query = query.Where(c => c.IsBlocked == isBlocked.Value);

            var customers = await query.OrderBy(c => c.CustomerName).ToListAsync();

            var headers = new[] { "Code", "Name", "Phone", "Credit Limit", "Balance", "Available", "Status" };
            var rows = customers.Select(c => new[]
            {
                c.CustomerCode,
                c.CustomerName,
                c.Phone,
                c.CreditLimit.ToString("N2"),
                c.CurrentBalance.ToString("N2"),
                Math.Max(0, c.CreditLimit - c.CurrentBalance).ToString("N2"),
                c.IsBlocked ? "Blocked" : (c.IsActive ? "Active" : "Inactive")
            }).ToList();

            var summary = new Dictionary<string, string>
            {
                { "Total Customers", customers.Count.ToString() },
                { "Active", customers.Count(c => c.IsActive && !c.IsBlocked).ToString() },
                { "Blocked", customers.Count(c => c.IsBlocked).ToString() },
                { "Total Outstanding", customers.Sum(c => c.CurrentBalance).ToString("N2") },
                { "Total Credit Limit", customers.Sum(c => c.CreditLimit).ToString("N2") }
            };

            return GenerateExport("Credit Customers List", headers, rows, summary, format, "credit-customers");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting credit customers");
            return StatusCode(500, "Failed to export customers");
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

    // Helper methods
    private static CreditCustomerDto MapToDto(CreditCustomer customer)
    {
        return new CreditCustomerDto
        {
            CreditCustomerId = customer.CreditCustomerId,
            CustomerCode = customer.CustomerCode,
            CustomerName = customer.CustomerName,
            ContactPerson = customer.ContactPerson,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            CreditLimit = customer.CreditLimit,
            CurrentBalance = customer.CurrentBalance,
            AvailableCredit = Math.Max(0, customer.CreditLimit - customer.CurrentBalance),
            PaymentTermDays = customer.PaymentTermDays,
            IsActive = customer.IsActive,
            IsBlocked = customer.IsBlocked,
            BlockReason = customer.BlockReason,
            VehicleNumbers = customer.VehicleNumbers,
            Notes = customer.Notes,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    private async Task<CreditTransactionDto> MapTransactionToDto(CreditTransaction transaction)
    {
        string? customerName = null;
        if (transaction.CreditCustomer != null)
        {
            customerName = transaction.CreditCustomer.CustomerName;
        }
        else
        {
            var customer = await _dbContext.CreditCustomers.FindAsync(transaction.CreditCustomerId);
            customerName = customer?.CustomerName ?? "Unknown";
        }

        return new CreditTransactionDto
        {
            CreditTransactionId = transaction.CreditTransactionId,
            CreditCustomerId = transaction.CreditCustomerId,
            CustomerName = customerName ?? "Unknown",
            TransactionType = transaction.TransactionType,
            TransactionTypeName = transaction.TransactionType.ToString(),
            Amount = transaction.Amount,
            BalanceAfter = transaction.BalanceAfter,
            TransactionDate = transaction.TransactionDate,
            FuelSaleId = transaction.FuelSaleId,
            SaleNumber = transaction.FuelSale?.SaleNumber,
            PaymentMode = transaction.PaymentMode,
            PaymentModeName = transaction.PaymentMode?.ToString(),
            PaymentReference = transaction.PaymentReference,
            Notes = transaction.Notes,
            CreatedBy = transaction.CreatedBy,
            CreatedAt = transaction.CreatedAt
        };
    }
}
