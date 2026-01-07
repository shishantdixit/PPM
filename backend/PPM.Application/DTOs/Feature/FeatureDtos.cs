namespace PPM.Application.DTOs.Feature;

/// <summary>
/// Basic feature information
/// </summary>
public class FeatureDto
{
    public Guid FeatureId { get; set; }
    public string FeatureCode { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Feature with plan default state
/// </summary>
public class PlanFeatureDto
{
    public Guid FeatureId { get; set; }
    public string FeatureCode { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Icon { get; set; }
    public bool IsEnabled { get; set; }
}

/// <summary>
/// Feature with tenant-specific state and override info
/// </summary>
public class TenantFeatureDto
{
    public Guid FeatureId { get; set; }
    public string FeatureCode { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Icon { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsOverridden { get; set; }
    public bool PlanDefault { get; set; }
    public string? OverriddenBy { get; set; }
    public DateTime? OverriddenAt { get; set; }
}

/// <summary>
/// For updating tenant feature overrides
/// </summary>
public class UpdateTenantFeatureDto
{
    public Guid FeatureId { get; set; }
    public bool IsEnabled { get; set; }
}

/// <summary>
/// Complete subscription info for owner view
/// </summary>
public class TenantSubscriptionDto
{
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string SubscriptionStatus { get; set; } = string.Empty;
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public int DaysRemaining { get; set; }
    public int MaxMachines { get; set; }
    public int MaxWorkers { get; set; }
    public int MaxMonthlyBills { get; set; }
    public List<TenantFeatureDto> Features { get; set; } = new();
}

/// <summary>
/// Feature access dictionary for frontend
/// </summary>
public class FeatureAccessDto
{
    public Dictionary<string, bool> Features { get; set; } = new();
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string? RequiredPlanForUpgrade { get; set; }
}
