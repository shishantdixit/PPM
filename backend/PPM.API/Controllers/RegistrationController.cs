using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.Application.Common;
using PPM.Application.DTOs.Registration;
using PPM.Application.Interfaces;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

/// <summary>
/// Public registration controller - no authentication required
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegistrationController> _logger;

    // Trial configuration
    private const int TrialDays = 7;
    private const int TrialMaxMachines = 2;
    private const int TrialMaxWorkers = 5;
    private const int TrialMaxMonthlyBills = 500;

    public RegistrationController(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        ILogger<RegistrationController> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Register a new tenant with a 7-day free trial
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] PublicRegistrationDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse($"Validation failed: {string.Join(", ", errors)}"));
        }

        // Normalize inputs
        var email = dto.Email.ToLower().Trim();
        var tenantCode = string.IsNullOrWhiteSpace(dto.TenantCode)
            ? await GenerateTenantCodeAsync(dto.CompanyName)
            : dto.TenantCode.ToUpper().Trim();

        // Check email uniqueness
        if (await _dbContext.Tenants.AnyAsync(t => t.Email.ToLower() == email))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Email address is already registered"));
        }

        // Check tenant code uniqueness
        if (await _dbContext.Tenants.AnyAsync(t => t.TenantCode == tenantCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Tenant code is already taken"));
        }

        var now = DateTime.UtcNow;
        var trialEndDate = now.AddDays(TrialDays);

        // Create tenant
        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            TenantCode = tenantCode,
            CompanyName = dto.CompanyName.Trim(),
            OwnerName = dto.OwnerName.Trim(),
            Email = email,
            Phone = dto.Phone.Trim(),
            Address = dto.Address?.Trim(),
            City = dto.City?.Trim(),
            State = dto.State?.Trim(),
            PinCode = dto.PinCode?.Trim(),
            Country = "India",

            // Trial subscription
            IsTrial = true,
            TrialStartDate = now,
            TrialEndDate = trialEndDate,
            SubscriptionPlan = "Basic",
            SubscriptionStatus = "Trial",
            SubscriptionStartDate = now,
            SubscriptionEndDate = trialEndDate,

            // Trial limits
            MaxMachines = TrialMaxMachines,
            MaxWorkers = TrialMaxWorkers,
            MaxMonthlyBills = TrialMaxMonthlyBills,

            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Create owner user
        var owner = new User
        {
            UserId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            Username = dto.Username.ToLower().Trim(),
            Email = email,
            Phone = dto.Phone.Trim(),
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            FullName = dto.OwnerName.Trim(),
            Role = "Owner",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            _dbContext.Tenants.Add(tenant);
            _dbContext.Users.Add(owner);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("New tenant registered: {TenantCode} ({CompanyName}) - Trial expires: {TrialEndDate}",
                tenantCode, tenant.CompanyName, trialEndDate);

            var response = new RegistrationResponseDto
            {
                TenantId = tenant.TenantId,
                TenantCode = tenant.TenantCode,
                CompanyName = tenant.CompanyName,
                Username = owner.Username,
                TrialStartDate = tenant.TrialStartDate ?? now,
                TrialEndDate = tenant.TrialEndDate ?? trialEndDate,
                TrialDaysRemaining = TrialDays,
                Message = $"Registration successful! Your 7-day free trial has started. Use tenant code '{tenantCode}' to login."
            };

            return Ok(ApiResponse<RegistrationResponseDto>.SuccessResponse(response, "Registration successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering tenant: {Email}", email);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during registration. Please try again."));
        }
    }

    /// <summary>
    /// Check if an email address is available for registration
    /// </summary>
    [HttpGet("check-email/{email}")]
    public async Task<IActionResult> CheckEmailAvailability(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(ApiResponse<AvailabilityCheckDto>.ErrorResponse("Email is required"));
        }

        var normalizedEmail = email.ToLower().Trim();
        var isAvailable = !await _dbContext.Tenants.AnyAsync(t => t.Email.ToLower() == normalizedEmail);

        return Ok(ApiResponse<AvailabilityCheckDto>.SuccessResponse(new AvailabilityCheckDto
        {
            IsAvailable = isAvailable,
            Message = isAvailable ? "Email is available" : "Email is already registered"
        }));
    }

    /// <summary>
    /// Check if a tenant code is available for registration
    /// </summary>
    [HttpGet("check-tenant-code/{code}")]
    public async Task<IActionResult> CheckTenantCodeAvailability(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(ApiResponse<AvailabilityCheckDto>.ErrorResponse("Tenant code is required"));
        }

        var normalizedCode = code.ToUpper().Trim();

        // Validate format
        if (!System.Text.RegularExpressions.Regex.IsMatch(normalizedCode, @"^[A-Z0-9]+$"))
        {
            return Ok(ApiResponse<AvailabilityCheckDto>.SuccessResponse(new AvailabilityCheckDto
            {
                IsAvailable = false,
                Message = "Tenant code can only contain letters and numbers"
            }));
        }

        if (normalizedCode.Length < 3 || normalizedCode.Length > 50)
        {
            return Ok(ApiResponse<AvailabilityCheckDto>.SuccessResponse(new AvailabilityCheckDto
            {
                IsAvailable = false,
                Message = "Tenant code must be between 3 and 50 characters"
            }));
        }

        var isAvailable = !await _dbContext.Tenants.AnyAsync(t => t.TenantCode == normalizedCode);

        return Ok(ApiResponse<AvailabilityCheckDto>.SuccessResponse(new AvailabilityCheckDto
        {
            IsAvailable = isAvailable,
            Message = isAvailable ? "Tenant code is available" : "Tenant code is already taken",
            SuggestedValue = isAvailable ? null : await GenerateTenantCodeAsync(normalizedCode)
        }));
    }

    /// <summary>
    /// Generate a suggested tenant code based on company name
    /// </summary>
    [HttpGet("suggest-tenant-code")]
    public async Task<IActionResult> SuggestTenantCode([FromQuery] string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Company name is required"));
        }

        var suggestedCode = await GenerateTenantCodeAsync(companyName);

        return Ok(ApiResponse<object>.SuccessResponse(new { tenantCode = suggestedCode }));
    }

    /// <summary>
    /// Generate a unique tenant code based on company name
    /// </summary>
    private async Task<string> GenerateTenantCodeAsync(string baseName)
    {
        // Clean up the name and create a base code
        var cleanName = new string(baseName.ToUpper()
            .Where(c => char.IsLetterOrDigit(c))
            .Take(6)
            .ToArray());

        if (cleanName.Length < 3)
        {
            cleanName = cleanName.PadRight(3, 'X');
        }

        // Try base code first
        if (!await _dbContext.Tenants.AnyAsync(t => t.TenantCode == cleanName))
        {
            return cleanName;
        }

        // Add numbers until unique
        var random = new Random();
        for (int i = 0; i < 100; i++)
        {
            var suffix = random.Next(100, 999).ToString();
            var code = $"{cleanName}{suffix}";

            if (!await _dbContext.Tenants.AnyAsync(t => t.TenantCode == code))
            {
                return code;
            }
        }

        // Fallback to GUID-based code
        return $"T{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}
