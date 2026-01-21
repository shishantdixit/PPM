using System.ComponentModel.DataAnnotations;

namespace PPM.Application.DTOs.License;

/// <summary>
/// DTO for generating a new license key (Super Admin)
/// </summary>
public class GenerateLicenseKeyDto
{
    [Required(ErrorMessage = "Subscription plan is required")]
    [RegularExpression("^(Basic|Premium|Enterprise)$", ErrorMessage = "Invalid subscription plan")]
    public string SubscriptionPlan { get; set; } = "Basic";

    [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 months")]
    public int DurationMonths { get; set; } = 12;

    [Range(1, 100, ErrorMessage = "Max machines must be between 1 and 100")]
    public int? MaxMachines { get; set; }

    [Range(1, 500, ErrorMessage = "Max workers must be between 1 and 500")]
    public int? MaxWorkers { get; set; }

    [Range(100, 1000000, ErrorMessage = "Max monthly bills must be between 100 and 1,000,000")]
    public int? MaxMonthlyBills { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for batch generating license keys
/// </summary>
public class BatchGenerateLicenseKeyDto
{
    [Range(1, 100, ErrorMessage = "Count must be between 1 and 100")]
    public int Count { get; set; } = 1;

    [Required(ErrorMessage = "Subscription plan is required")]
    [RegularExpression("^(Basic|Premium|Enterprise)$", ErrorMessage = "Invalid subscription plan")]
    public string SubscriptionPlan { get; set; } = "Basic";

    [Range(1, 60, ErrorMessage = "Duration must be between 1 and 60 months")]
    public int DurationMonths { get; set; } = 12;

    [Range(1, 100, ErrorMessage = "Max machines must be between 1 and 100")]
    public int? MaxMachines { get; set; }

    [Range(1, 500, ErrorMessage = "Max workers must be between 1 and 500")]
    public int? MaxWorkers { get; set; }

    [Range(100, 1000000, ErrorMessage = "Max monthly bills must be between 100 and 1,000,000")]
    public int? MaxMonthlyBills { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

/// <summary>
/// License key response DTO
/// </summary>
public class LicenseKeyDto
{
    public Guid LicenseKeyId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string SubscriptionPlan { get; set; } = string.Empty;
    public int DurationMonths { get; set; }
    public int MaxMachines { get; set; }
    public int MaxWorkers { get; set; }
    public int MaxMonthlyBills { get; set; }
    public string Status { get; set; } = string.Empty;

    // Activation info (if used)
    public Guid? ActivatedByTenantId { get; set; }
    public string? ActivatedByTenantCode { get; set; }
    public string? ActivatedByCompanyName { get; set; }
    public DateTime? ActivatedAt { get; set; }

    // Admin tracking
    public string GeneratedByUsername { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for license key activation (by tenant owner)
/// </summary>
public class ActivateLicenseKeyDto
{
    [Required(ErrorMessage = "License key is required")]
    [RegularExpression(@"^PPM-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$",
        ErrorMessage = "Invalid license key format. Expected format: PPM-XXXX-XXXX-XXXX-XXXX")]
    public string LicenseKey { get; set; } = string.Empty;
}

/// <summary>
/// License activation result
/// </summary>
public class ActivationResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? NewSubscriptionPlan { get; set; }
    public DateTime? NewSubscriptionEndDate { get; set; }
    public int? MaxMachines { get; set; }
    public int? MaxWorkers { get; set; }
    public int? MaxMonthlyBills { get; set; }
}

/// <summary>
/// DTO for revoking a license key
/// </summary>
public class RevokeLicenseKeyDto
{
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string? Reason { get; set; }
}

/// <summary>
/// License keys list query parameters
/// </summary>
public class LicenseKeyQueryDto
{
    public string? Status { get; set; } // Available, Used, Revoked
    public string? SubscriptionPlan { get; set; }
    public string? Search { get; set; } // Search by key or tenant code
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}

/// <summary>
/// Subscription status response (for tenant owners)
/// </summary>
public class SubscriptionStatusDto
{
    public Guid TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    // Trial info
    public bool IsTrial { get; set; }
    public DateTime? TrialStartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public int? TrialDaysRemaining { get; set; }

    // Subscription info
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string SubscriptionStatus { get; set; } = string.Empty;
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public int? DaysRemaining { get; set; }

    // Limits
    public int MaxMachines { get; set; }
    public int MaxWorkers { get; set; }
    public int MaxMonthlyBills { get; set; }

    // Current usage
    public int CurrentMachineCount { get; set; }
    public int CurrentWorkerCount { get; set; }
    public int CurrentMonthBillCount { get; set; }

    // License info
    public Guid? ActiveLicenseKeyId { get; set; }
    public DateTime? LicenseActivatedAt { get; set; }
}
