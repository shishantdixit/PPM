using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Defines default feature availability for each subscription plan
/// </summary>
public class PlanFeature : BaseEntity
{
    public Guid PlanFeatureId { get; set; }

    /// <summary>
    /// Subscription plan name ("Basic", "Premium", "Enterprise")
    /// </summary>
    public string SubscriptionPlan { get; set; } = string.Empty;

    /// <summary>
    /// Reference to the feature
    /// </summary>
    public Guid FeatureId { get; set; }

    /// <summary>
    /// Whether this feature is enabled by default for this plan
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    // Navigation
    public virtual Feature Feature { get; set; } = null!;
}
