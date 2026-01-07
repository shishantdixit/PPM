using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPM.Application.DTOs.Feature;
using PPM.Application.Interfaces;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.Infrastructure.Services;

/// <summary>
/// Service for managing feature access and subscriptions
/// </summary>
public class FeatureService : IFeatureService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(ApplicationDbContext dbContext, ILogger<FeatureService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<FeatureDto>> GetAllFeaturesAsync()
    {
        return await _dbContext.Features
            .Where(f => f.IsActive)
            .OrderBy(f => f.DisplayOrder)
            .Select(f => new FeatureDto
            {
                FeatureId = f.FeatureId,
                FeatureCode = f.FeatureCode,
                FeatureName = f.FeatureName,
                Description = f.Description,
                Module = f.Module,
                Icon = f.Icon,
                DisplayOrder = f.DisplayOrder,
                IsActive = f.IsActive
            })
            .ToListAsync();
    }

    public async Task<List<PlanFeatureDto>> GetPlanFeaturesAsync(string plan)
    {
        return await _dbContext.PlanFeatures
            .Where(pf => pf.SubscriptionPlan == plan)
            .Include(pf => pf.Feature)
            .Where(pf => pf.Feature.IsActive)
            .OrderBy(pf => pf.Feature.DisplayOrder)
            .Select(pf => new PlanFeatureDto
            {
                FeatureId = pf.FeatureId,
                FeatureCode = pf.Feature.FeatureCode,
                FeatureName = pf.Feature.FeatureName,
                Description = pf.Feature.Description,
                Module = pf.Feature.Module,
                Icon = pf.Feature.Icon,
                IsEnabled = pf.IsEnabled
            })
            .ToListAsync();
    }

    public async Task<List<TenantFeatureDto>> GetTenantFeaturesAsync(Guid tenantId)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            return new List<TenantFeatureDto>();
        }

        // Get all active features
        var features = await _dbContext.Features
            .Where(f => f.IsActive)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();

        // Get plan defaults
        var planFeatures = await _dbContext.PlanFeatures
            .Where(pf => pf.SubscriptionPlan == tenant.SubscriptionPlan)
            .ToDictionaryAsync(pf => pf.FeatureId, pf => pf.IsEnabled);

        // Get tenant-specific overrides
        var tenantFeatures = await _dbContext.TenantFeatures
            .Where(tf => tf.TenantId == tenantId)
            .ToDictionaryAsync(tf => tf.FeatureId, tf => tf);

        var result = new List<TenantFeatureDto>();

        foreach (var feature in features)
        {
            var planDefault = planFeatures.GetValueOrDefault(feature.FeatureId, false);
            var tenantOverride = tenantFeatures.GetValueOrDefault(feature.FeatureId);

            result.Add(new TenantFeatureDto
            {
                FeatureId = feature.FeatureId,
                FeatureCode = feature.FeatureCode,
                FeatureName = feature.FeatureName,
                Description = feature.Description,
                Module = feature.Module,
                Icon = feature.Icon,
                IsEnabled = tenantOverride?.IsOverridden == true ? tenantOverride.IsEnabled : planDefault,
                IsOverridden = tenantOverride?.IsOverridden ?? false,
                PlanDefault = planDefault,
                OverriddenBy = tenantOverride?.OverriddenBy,
                OverriddenAt = tenantOverride?.OverriddenAt
            });
        }

        return result;
    }

    public async Task<FeatureAccessDto> GetTenantFeatureAccessAsync(Guid tenantId)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            return new FeatureAccessDto();
        }

        var tenantFeatures = await GetTenantFeaturesAsync(tenantId);

        return new FeatureAccessDto
        {
            Features = tenantFeatures.ToDictionary(f => f.FeatureCode, f => f.IsEnabled),
            SubscriptionPlan = tenant.SubscriptionPlan,
            RequiredPlanForUpgrade = GetUpgradePlan(tenant.SubscriptionPlan)
        };
    }

    public async Task<bool> HasFeatureAccessAsync(Guid tenantId, string featureCode)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            return false;
        }

        var feature = await _dbContext.Features
            .FirstOrDefaultAsync(f => f.FeatureCode == featureCode && f.IsActive);

        if (feature == null)
        {
            return false;
        }

        // Check for tenant-specific override first
        var tenantOverride = await _dbContext.TenantFeatures
            .FirstOrDefaultAsync(tf => tf.TenantId == tenantId && tf.FeatureId == feature.FeatureId && tf.IsOverridden);

        if (tenantOverride != null)
        {
            return tenantOverride.IsEnabled;
        }

        // Fall back to plan default
        var planFeature = await _dbContext.PlanFeatures
            .FirstOrDefaultAsync(pf => pf.SubscriptionPlan == tenant.SubscriptionPlan && pf.FeatureId == feature.FeatureId);

        return planFeature?.IsEnabled ?? false;
    }

    public async Task<List<TenantFeatureDto>> UpdateTenantFeaturesAsync(Guid tenantId, List<UpdateTenantFeatureDto> updates, string adminUsername)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            throw new InvalidOperationException("Tenant not found");
        }

        // Get plan defaults for comparison
        var planFeatures = await _dbContext.PlanFeatures
            .Where(pf => pf.SubscriptionPlan == tenant.SubscriptionPlan)
            .ToDictionaryAsync(pf => pf.FeatureId, pf => pf.IsEnabled);

        foreach (var update in updates)
        {
            var existingOverride = await _dbContext.TenantFeatures
                .FirstOrDefaultAsync(tf => tf.TenantId == tenantId && tf.FeatureId == update.FeatureId);

            var planDefault = planFeatures.GetValueOrDefault(update.FeatureId, false);
            var isOverriding = update.IsEnabled != planDefault;

            if (existingOverride != null)
            {
                if (isOverriding)
                {
                    existingOverride.IsEnabled = update.IsEnabled;
                    existingOverride.IsOverridden = true;
                    existingOverride.OverriddenBy = adminUsername;
                    existingOverride.OverriddenAt = DateTime.UtcNow;
                    existingOverride.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Remove override if setting back to plan default
                    _dbContext.TenantFeatures.Remove(existingOverride);
                }
            }
            else if (isOverriding)
            {
                // Create new override
                _dbContext.TenantFeatures.Add(new TenantFeature
                {
                    TenantFeatureId = Guid.NewGuid(),
                    TenantId = tenantId,
                    FeatureId = update.FeatureId,
                    IsEnabled = update.IsEnabled,
                    IsOverridden = true,
                    OverriddenBy = adminUsername,
                    OverriddenAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Updated features for tenant {TenantId} by {Admin}", tenantId, adminUsername);

        return await GetTenantFeaturesAsync(tenantId);
    }

    public async Task ResetTenantFeaturesToPlanDefaultsAsync(Guid tenantId)
    {
        var tenantOverrides = await _dbContext.TenantFeatures
            .Where(tf => tf.TenantId == tenantId)
            .ToListAsync();

        _dbContext.TenantFeatures.RemoveRange(tenantOverrides);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Reset features to plan defaults for tenant {TenantId}", tenantId);
    }

    public async Task<TenantSubscriptionDto?> GetTenantSubscriptionAsync(Guid tenantId)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            return null;
        }

        var features = await GetTenantFeaturesAsync(tenantId);

        var daysRemaining = tenant.SubscriptionEndDate.HasValue
            ? (int)(tenant.SubscriptionEndDate.Value - DateTime.UtcNow).TotalDays
            : -1; // -1 indicates no expiration

        return new TenantSubscriptionDto
        {
            SubscriptionPlan = tenant.SubscriptionPlan,
            SubscriptionStatus = tenant.SubscriptionStatus,
            SubscriptionStartDate = tenant.SubscriptionStartDate,
            SubscriptionEndDate = tenant.SubscriptionEndDate,
            DaysRemaining = Math.Max(0, daysRemaining),
            MaxMachines = tenant.MaxMachines,
            MaxWorkers = tenant.MaxWorkers,
            MaxMonthlyBills = tenant.MaxMonthlyBills,
            Features = features
        };
    }

    private static string? GetUpgradePlan(string currentPlan)
    {
        return currentPlan switch
        {
            "Basic" => "Premium",
            "Premium" => "Enterprise",
            "Enterprise" => null, // Already on highest plan
            _ => "Premium"
        };
    }
}
