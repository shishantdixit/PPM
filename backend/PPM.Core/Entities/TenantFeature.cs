using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Per-tenant feature override. Allows Super Admin to customize
/// features for individual tenants beyond their plan defaults.
/// </summary>
public class TenantFeature : BaseEntity
{
    public Guid TenantFeatureId { get; set; }

    /// <summary>
    /// The tenant this override applies to
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The feature being overridden
    /// </summary>
    public Guid FeatureId { get; set; }

    /// <summary>
    /// Whether the feature is enabled for this tenant
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// True if this was manually overridden from plan default
    /// </summary>
    public bool IsOverridden { get; set; }

    /// <summary>
    /// Username of Super Admin who made the override
    /// </summary>
    public string? OverriddenBy { get; set; }

    /// <summary>
    /// When the override was made
    /// </summary>
    public DateTime? OverriddenAt { get; set; }

    // Navigation
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Feature Feature { get; set; } = null!;
}
