using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Represents a feature that can be enabled/disabled per subscription plan
/// </summary>
public class Feature : BaseEntity
{
    public Guid FeatureId { get; set; }

    /// <summary>
    /// Unique code for the feature (e.g., "REPORTS", "CREDIT_CUSTOMERS")
    /// </summary>
    public string FeatureCode { get; set; } = string.Empty;

    /// <summary>
    /// Display name (e.g., "Reports & Analytics")
    /// </summary>
    public string FeatureName { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the feature provides
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Module grouping (e.g., "Analytics", "Sales", "Finance")
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// Icon identifier for UI display
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Order for consistent UI display
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// System flag to enable/disable feature globally
    /// </summary>
    public bool IsActive { get; set; } = true;
}
