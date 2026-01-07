using PPM.Application.DTOs.Feature;

namespace PPM.Application.Interfaces;

/// <summary>
/// Service for managing feature access and subscriptions
/// </summary>
public interface IFeatureService
{
    /// <summary>
    /// Get all available features
    /// </summary>
    Task<List<FeatureDto>> GetAllFeaturesAsync();

    /// <summary>
    /// Get features for a specific subscription plan
    /// </summary>
    Task<List<PlanFeatureDto>> GetPlanFeaturesAsync(string plan);

    /// <summary>
    /// Get effective features for a tenant (plan defaults + overrides)
    /// </summary>
    Task<List<TenantFeatureDto>> GetTenantFeaturesAsync(Guid tenantId);

    /// <summary>
    /// Get feature access dictionary for a tenant
    /// </summary>
    Task<FeatureAccessDto> GetTenantFeatureAccessAsync(Guid tenantId);

    /// <summary>
    /// Check if a tenant has access to a specific feature
    /// </summary>
    Task<bool> HasFeatureAccessAsync(Guid tenantId, string featureCode);

    /// <summary>
    /// Update tenant-specific feature overrides
    /// </summary>
    Task<List<TenantFeatureDto>> UpdateTenantFeaturesAsync(Guid tenantId, List<UpdateTenantFeatureDto> updates, string adminUsername);

    /// <summary>
    /// Reset tenant features to plan defaults
    /// </summary>
    Task ResetTenantFeaturesToPlanDefaultsAsync(Guid tenantId);

    /// <summary>
    /// Get complete subscription info for a tenant (for owner view)
    /// </summary>
    Task<TenantSubscriptionDto?> GetTenantSubscriptionAsync(Guid tenantId);
}
