using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PPM.Application.DTOs.Feature;
using PPM.Core.Entities;
using PPM.Infrastructure.Services;
using PPM.Tests.Helpers;

namespace PPM.Tests.Services;

public class FeatureServiceTests : IDisposable
{
    private readonly Mock<ILogger<FeatureService>> _loggerMock;

    public FeatureServiceTests()
    {
        _loggerMock = new Mock<ILogger<FeatureService>>();
    }

    public void Dispose()
    {
        // Cleanup if needed
    }

    #region GetAllFeatures Tests

    [Fact]
    public async Task GetAllFeaturesAsync_ReturnsOnlyActiveFeatures()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var features = await service.GetAllFeaturesAsync();

        // Assert
        features.Should().NotBeEmpty();
        features.Should().OnlyContain(f => f.IsActive);
    }

    [Fact]
    public async Task GetAllFeaturesAsync_ReturnsOrderedByDisplayOrder()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);

        // Add features with specific display orders
        context.Features.AddRange(new[]
        {
            new Feature
            {
                FeatureId = Guid.NewGuid(),
                FeatureCode = "FEAT_C",
                FeatureName = "Feature C",
                Description = "Third",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = Guid.NewGuid(),
                FeatureCode = "FEAT_A",
                FeatureName = "Feature A",
                Description = "First",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Feature
            {
                FeatureId = Guid.NewGuid(),
                FeatureCode = "FEAT_B",
                FeatureName = "Feature B",
                Description = "Second",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        });
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var features = await service.GetAllFeaturesAsync();

        // Assert
        features.Should().HaveCount(3);
        features[0].FeatureCode.Should().Be("FEAT_A");
        features[1].FeatureCode.Should().Be("FEAT_B");
        features[2].FeatureCode.Should().Be("FEAT_C");
    }

    #endregion

    #region GetPlanFeatures Tests

    [Fact]
    public async Task GetPlanFeaturesAsync_ReturnsFeaturesForPlan()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var premiumFeatures = await service.GetPlanFeaturesAsync("Premium");
        var basicFeatures = await service.GetPlanFeaturesAsync("Basic");

        // Assert
        premiumFeatures.Should().NotBeEmpty();
        basicFeatures.Should().NotBeEmpty();

        // Premium should have REPORTS enabled
        var premiumReports = premiumFeatures.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        premiumReports.Should().NotBeNull();
        premiumReports!.IsEnabled.Should().BeTrue();

        // Basic should have REPORTS disabled
        var basicReports = basicFeatures.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        basicReports.Should().NotBeNull();
        basicReports!.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task GetPlanFeaturesAsync_ReturnsEmptyForUnknownPlan()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var features = await service.GetPlanFeaturesAsync("NonExistentPlan");

        // Assert
        features.Should().BeEmpty();
    }

    #endregion

    #region GetTenantFeatures Tests

    [Fact]
    public async Task GetTenantFeaturesAsync_ReturnsPlanDefaultsWhenNoOverrides()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var features = await service.GetTenantFeaturesAsync(TestDbContextFactory.TestTenantId);

        // Assert
        features.Should().NotBeEmpty();

        // Test tenant 1 is on Premium plan
        var reportsFeature = features.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        reportsFeature.Should().NotBeNull();
        reportsFeature!.IsEnabled.Should().BeTrue(); // Premium has REPORTS enabled
        reportsFeature.IsOverridden.Should().BeFalse();
        reportsFeature.PlanDefault.Should().BeTrue();
    }

    [Fact]
    public async Task GetTenantFeaturesAsync_ReturnsEmptyForNonExistentTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var features = await service.GetTenantFeaturesAsync(Guid.NewGuid());

        // Assert
        features.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTenantFeaturesAsync_ReturnsOverriddenValues()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        // Get the REPORTS feature ID
        var reportsFeature = context.Features.First(f => f.FeatureCode == "REPORTS");

        // Add override for tenant 2 (Basic plan) - enable REPORTS
        context.TenantFeatures.Add(new TenantFeature
        {
            TenantFeatureId = Guid.NewGuid(),
            TenantId = TestDbContextFactory.TestTenant2Id,
            FeatureId = reportsFeature.FeatureId,
            IsEnabled = true,
            IsOverridden = true,
            OverriddenBy = "admin",
            OverriddenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var features = await service.GetTenantFeaturesAsync(TestDbContextFactory.TestTenant2Id);

        // Assert
        var feature = features.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        feature.Should().NotBeNull();
        feature!.IsEnabled.Should().BeTrue(); // Overridden to true
        feature.IsOverridden.Should().BeTrue();
        feature.PlanDefault.Should().BeFalse(); // Basic plan default is false
        feature.OverriddenBy.Should().Be("admin");
    }

    #endregion

    #region HasFeatureAccess Tests

    [Fact]
    public async Task HasFeatureAccessAsync_ReturnsTrueForPlanEnabledFeature()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act - Tenant 1 is Premium, REPORTS is enabled for Premium
        var hasAccess = await service.HasFeatureAccessAsync(TestDbContextFactory.TestTenantId, "REPORTS");

        // Assert
        hasAccess.Should().BeTrue();
    }

    [Fact]
    public async Task HasFeatureAccessAsync_ReturnsFalseForPlanDisabledFeature()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act - Tenant 2 is Basic, REPORTS is disabled for Basic
        var hasAccess = await service.HasFeatureAccessAsync(TestDbContextFactory.TestTenant2Id, "REPORTS");

        // Assert
        hasAccess.Should().BeFalse();
    }

    [Fact]
    public async Task HasFeatureAccessAsync_RespectsOverrides()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        var reportsFeature = context.Features.First(f => f.FeatureCode == "REPORTS");

        // Override: Enable REPORTS for Tenant 2 (Basic plan)
        context.TenantFeatures.Add(new TenantFeature
        {
            TenantFeatureId = Guid.NewGuid(),
            TenantId = TestDbContextFactory.TestTenant2Id,
            FeatureId = reportsFeature.FeatureId,
            IsEnabled = true,
            IsOverridden = true,
            OverriddenBy = "admin",
            OverriddenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var hasAccess = await service.HasFeatureAccessAsync(TestDbContextFactory.TestTenant2Id, "REPORTS");

        // Assert
        hasAccess.Should().BeTrue(); // Override takes precedence
    }

    [Fact]
    public async Task HasFeatureAccessAsync_ReturnsFalseForNonExistentFeature()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var hasAccess = await service.HasFeatureAccessAsync(TestDbContextFactory.TestTenantId, "NON_EXISTENT_FEATURE");

        // Assert
        hasAccess.Should().BeFalse();
    }

    [Fact]
    public async Task HasFeatureAccessAsync_ReturnsFalseForNonExistentTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var hasAccess = await service.HasFeatureAccessAsync(Guid.NewGuid(), "REPORTS");

        // Assert
        hasAccess.Should().BeFalse();
    }

    #endregion

    #region GetTenantFeatureAccess Tests

    [Fact]
    public async Task GetTenantFeatureAccessAsync_ReturnsFeatureDictionary()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var access = await service.GetTenantFeatureAccessAsync(TestDbContextFactory.TestTenantId);

        // Assert
        access.Should().NotBeNull();
        access.Features.Should().ContainKey("REPORTS");
        access.Features["REPORTS"].Should().BeTrue();
        access.SubscriptionPlan.Should().Be("Premium");
        access.RequiredPlanForUpgrade.Should().Be("Enterprise");
    }

    [Fact]
    public async Task GetTenantFeatureAccessAsync_ReturnsEmptyForNonExistentTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var access = await service.GetTenantFeatureAccessAsync(Guid.NewGuid());

        // Assert
        access.Features.Should().BeEmpty();
    }

    #endregion

    #region UpdateTenantFeatures Tests

    [Fact]
    public async Task UpdateTenantFeaturesAsync_CreatesOverride()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        var reportsFeature = context.Features.First(f => f.FeatureCode == "REPORTS");
        var service = new FeatureService(context, _loggerMock.Object);

        var updates = new List<UpdateTenantFeatureDto>
        {
            new UpdateTenantFeatureDto
            {
                FeatureId = reportsFeature.FeatureId,
                IsEnabled = true // Basic plan has this disabled, so this is an override
            }
        };

        // Act
        var result = await service.UpdateTenantFeaturesAsync(TestDbContextFactory.TestTenant2Id, updates, "admin");

        // Assert
        var feature = result.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        feature.Should().NotBeNull();
        feature!.IsEnabled.Should().BeTrue();
        feature.IsOverridden.Should().BeTrue();
        feature.OverriddenBy.Should().Be("admin");
    }

    [Fact]
    public async Task UpdateTenantFeaturesAsync_RemovesOverrideWhenMatchesPlanDefault()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        var reportsFeature = context.Features.First(f => f.FeatureCode == "REPORTS");

        // First create an override
        context.TenantFeatures.Add(new TenantFeature
        {
            TenantFeatureId = Guid.NewGuid(),
            TenantId = TestDbContextFactory.TestTenant2Id,
            FeatureId = reportsFeature.FeatureId,
            IsEnabled = true,
            IsOverridden = true,
            OverriddenBy = "admin",
            OverriddenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Now set it back to plan default (false for Basic)
        var updates = new List<UpdateTenantFeatureDto>
        {
            new UpdateTenantFeatureDto
            {
                FeatureId = reportsFeature.FeatureId,
                IsEnabled = false // Matches Basic plan default
            }
        };

        // Act
        var result = await service.UpdateTenantFeaturesAsync(TestDbContextFactory.TestTenant2Id, updates, "admin");

        // Assert
        var feature = result.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        feature.Should().NotBeNull();
        feature!.IsEnabled.Should().BeFalse();
        feature.IsOverridden.Should().BeFalse(); // Override removed

        // Verify override was removed from database
        var dbOverride = context.TenantFeatures.FirstOrDefault(
            tf => tf.TenantId == TestDbContextFactory.TestTenant2Id && tf.FeatureId == reportsFeature.FeatureId);
        dbOverride.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTenantFeaturesAsync_ThrowsForNonExistentTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        var updates = new List<UpdateTenantFeatureDto>();

        // Act & Assert
        await service.Invoking(s => s.UpdateTenantFeaturesAsync(Guid.NewGuid(), updates, "admin"))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Tenant not found");
    }

    #endregion

    #region ResetTenantFeatures Tests

    [Fact]
    public async Task ResetTenantFeaturesToPlanDefaultsAsync_RemovesAllOverrides()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        var reportsFeature = context.Features.First(f => f.FeatureCode == "REPORTS");

        // Create override
        context.TenantFeatures.Add(new TenantFeature
        {
            TenantFeatureId = Guid.NewGuid(),
            TenantId = TestDbContextFactory.TestTenant2Id,
            FeatureId = reportsFeature.FeatureId,
            IsEnabled = true,
            IsOverridden = true,
            OverriddenBy = "admin",
            OverriddenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        await service.ResetTenantFeaturesToPlanDefaultsAsync(TestDbContextFactory.TestTenant2Id);

        // Assert
        var overrides = context.TenantFeatures.Where(tf => tf.TenantId == TestDbContextFactory.TestTenant2Id).ToList();
        overrides.Should().BeEmpty();
    }

    #endregion

    #region GetTenantSubscription Tests

    [Fact]
    public async Task GetTenantSubscriptionAsync_ReturnsFullSubscriptionInfo()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var subscription = await service.GetTenantSubscriptionAsync(TestDbContextFactory.TestTenantId);

        // Assert
        subscription.Should().NotBeNull();
        subscription!.SubscriptionPlan.Should().Be("Premium");
        subscription.SubscriptionStatus.Should().Be("Active");
        subscription.MaxMachines.Should().Be(10);
        subscription.MaxWorkers.Should().Be(50);
        subscription.DaysRemaining.Should().BeGreaterThan(0);
        subscription.Features.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetTenantSubscriptionAsync_ReturnsNullForNonExistentTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);
        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var subscription = await service.GetTenantSubscriptionAsync(Guid.NewGuid());

        // Assert
        subscription.Should().BeNull();
    }

    [Fact]
    public async Task GetTenantSubscriptionAsync_CalculatesDaysRemainingCorrectly()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        // Update tenant subscription end date to a specific date
        var tenant = context.Tenants.First(t => t.TenantId == TestDbContextFactory.TestTenantId);
        tenant.SubscriptionEndDate = DateTime.UtcNow.AddDays(30);
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var subscription = await service.GetTenantSubscriptionAsync(TestDbContextFactory.TestTenantId);

        // Assert
        subscription!.DaysRemaining.Should().BeCloseTo(30, 1); // Allow 1 day tolerance
    }

    #endregion

    #region Multi-Tenant Isolation Tests

    [Fact]
    public async Task GetTenantFeaturesAsync_EnforcesTenantIsolation()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        var reportsFeature = context.Features.First(f => f.FeatureCode == "REPORTS");

        // Add override only for Tenant 2
        context.TenantFeatures.Add(new TenantFeature
        {
            TenantFeatureId = Guid.NewGuid(),
            TenantId = TestDbContextFactory.TestTenant2Id,
            FeatureId = reportsFeature.FeatureId,
            IsEnabled = true,
            IsOverridden = true,
            OverriddenBy = "admin",
            OverriddenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Act
        var tenant1Features = await service.GetTenantFeaturesAsync(TestDbContextFactory.TestTenantId);
        var tenant2Features = await service.GetTenantFeaturesAsync(TestDbContextFactory.TestTenant2Id);

        // Assert
        // Tenant 1 should not see Tenant 2's override
        var tenant1Reports = tenant1Features.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        tenant1Reports!.IsOverridden.Should().BeFalse();
        tenant1Reports.OverriddenBy.Should().BeNull();

        // Tenant 2 should see its override
        var tenant2Reports = tenant2Features.FirstOrDefault(f => f.FeatureCode == "REPORTS");
        tenant2Reports!.IsOverridden.Should().BeTrue();
        tenant2Reports.OverriddenBy.Should().Be("admin");
    }

    [Fact]
    public async Task ResetTenantFeaturesToPlanDefaultsAsync_OnlyAffectsTargetTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbContextFactory.Create(dbName);
        TestDbContextFactory.SeedTestData(context);

        var reportsFeature = context.Features.First(f => f.FeatureCode == "REPORTS");

        // Add overrides for both tenants
        context.TenantFeatures.AddRange(new[]
        {
            new TenantFeature
            {
                TenantFeatureId = Guid.NewGuid(),
                TenantId = TestDbContextFactory.TestTenantId,
                FeatureId = reportsFeature.FeatureId,
                IsEnabled = false,
                IsOverridden = true,
                OverriddenBy = "admin",
                OverriddenAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TenantFeature
            {
                TenantFeatureId = Guid.NewGuid(),
                TenantId = TestDbContextFactory.TestTenant2Id,
                FeatureId = reportsFeature.FeatureId,
                IsEnabled = true,
                IsOverridden = true,
                OverriddenBy = "admin",
                OverriddenAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        });
        await context.SaveChangesAsync();

        var service = new FeatureService(context, _loggerMock.Object);

        // Act - Reset only Tenant 1
        await service.ResetTenantFeaturesToPlanDefaultsAsync(TestDbContextFactory.TestTenantId);

        // Assert
        var tenant1Overrides = context.TenantFeatures.Where(tf => tf.TenantId == TestDbContextFactory.TestTenantId).ToList();
        var tenant2Overrides = context.TenantFeatures.Where(tf => tf.TenantId == TestDbContextFactory.TestTenant2Id).ToList();

        tenant1Overrides.Should().BeEmpty(); // Cleared
        tenant2Overrides.Should().HaveCount(1); // Not affected
    }

    #endregion
}
