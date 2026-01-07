using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.Feature;
using PPM.Application.Interfaces;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeaturesController : ControllerBase
{
    private readonly IFeatureService _featureService;
    private readonly ILogger<FeaturesController> _logger;

    public FeaturesController(IFeatureService featureService, ILogger<FeaturesController> logger)
    {
        _featureService = featureService;
        _logger = logger;
    }

    /// <summary>
    /// Get all available features (Super Admin only)
    /// </summary>
    [HttpGet]
    [RequireSuperAdmin]
    public async Task<IActionResult> GetAllFeatures()
    {
        try
        {
            var features = await _featureService.GetAllFeaturesAsync();
            return Ok(ApiResponse<List<FeatureDto>>.SuccessResponse(features));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching features");
            return StatusCode(500, ApiResponse<List<FeatureDto>>.ErrorResponse("Failed to fetch features"));
        }
    }

    /// <summary>
    /// Get features for a specific subscription plan (Super Admin only)
    /// </summary>
    [HttpGet("plans/{plan}")]
    [RequireSuperAdmin]
    public async Task<IActionResult> GetPlanFeatures(string plan)
    {
        try
        {
            var features = await _featureService.GetPlanFeaturesAsync(plan);
            return Ok(ApiResponse<List<PlanFeatureDto>>.SuccessResponse(features));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching plan features for {Plan}", plan);
            return StatusCode(500, ApiResponse<List<PlanFeatureDto>>.ErrorResponse("Failed to fetch plan features"));
        }
    }

    /// <summary>
    /// Get features for a specific tenant (Super Admin only)
    /// </summary>
    [HttpGet("tenant/{tenantId}")]
    [RequireSuperAdmin]
    public async Task<IActionResult> GetTenantFeatures(Guid tenantId)
    {
        try
        {
            var features = await _featureService.GetTenantFeaturesAsync(tenantId);
            return Ok(ApiResponse<List<TenantFeatureDto>>.SuccessResponse(features));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenant features for {TenantId}", tenantId);
            return StatusCode(500, ApiResponse<List<TenantFeatureDto>>.ErrorResponse("Failed to fetch tenant features"));
        }
    }

    /// <summary>
    /// Update feature overrides for a tenant (Super Admin only)
    /// </summary>
    [HttpPut("tenant/{tenantId}")]
    [RequireSuperAdmin]
    public async Task<IActionResult> UpdateTenantFeatures(Guid tenantId, [FromBody] List<UpdateTenantFeatureDto> updates)
    {
        try
        {
            var username = User.Identity?.Name ?? "system";
            var features = await _featureService.UpdateTenantFeaturesAsync(tenantId, updates, username);
            return Ok(ApiResponse<List<TenantFeatureDto>>.SuccessResponse(features, "Features updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<List<TenantFeatureDto>>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant features for {TenantId}", tenantId);
            return StatusCode(500, ApiResponse<List<TenantFeatureDto>>.ErrorResponse("Failed to update tenant features"));
        }
    }

    /// <summary>
    /// Reset tenant features to plan defaults (Super Admin only)
    /// </summary>
    [HttpPost("tenant/{tenantId}/reset")]
    [RequireSuperAdmin]
    public async Task<IActionResult> ResetTenantFeatures(Guid tenantId)
    {
        try
        {
            await _featureService.ResetTenantFeaturesToPlanDefaultsAsync(tenantId);
            var features = await _featureService.GetTenantFeaturesAsync(tenantId);
            return Ok(ApiResponse<List<TenantFeatureDto>>.SuccessResponse(features, "Features reset to plan defaults"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting tenant features for {TenantId}", tenantId);
            return StatusCode(500, ApiResponse<List<TenantFeatureDto>>.ErrorResponse("Failed to reset tenant features"));
        }
    }

    /// <summary>
    /// Get current tenant's feature access (for tenant users)
    /// </summary>
    [HttpGet("my-access")]
    public async Task<IActionResult> GetMyFeatureAccess()
    {
        try
        {
            var tenantContext = HttpContext.GetTenantContext();

            // Super admin has all features
            if (tenantContext?.IsSuperAdmin == true)
            {
                var allFeatures = await _featureService.GetAllFeaturesAsync();
                return Ok(ApiResponse<FeatureAccessDto>.SuccessResponse(new FeatureAccessDto
                {
                    Features = allFeatures.ToDictionary(f => f.FeatureCode, _ => true),
                    SubscriptionPlan = "Enterprise",
                    RequiredPlanForUpgrade = null
                }));
            }

            if (tenantContext?.TenantId == null)
            {
                return Unauthorized(ApiResponse<FeatureAccessDto>.ErrorResponse("Tenant context not found"));
            }

            var featureAccess = await _featureService.GetTenantFeatureAccessAsync(tenantContext.TenantId.Value);
            return Ok(ApiResponse<FeatureAccessDto>.SuccessResponse(featureAccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching feature access");
            return StatusCode(500, ApiResponse<FeatureAccessDto>.ErrorResponse("Failed to fetch feature access"));
        }
    }

    /// <summary>
    /// Get subscription details for owner view
    /// </summary>
    [HttpGet("subscription")]
    [RequireRole("Owner")]
    public async Task<IActionResult> GetMySubscription()
    {
        try
        {
            var tenantContext = HttpContext.GetTenantContext();

            if (tenantContext?.TenantId == null)
            {
                return Unauthorized(ApiResponse<TenantSubscriptionDto>.ErrorResponse("Tenant context not found"));
            }

            var subscription = await _featureService.GetTenantSubscriptionAsync(tenantContext.TenantId.Value);

            if (subscription == null)
            {
                return NotFound(ApiResponse<TenantSubscriptionDto>.ErrorResponse("Subscription not found"));
            }

            return Ok(ApiResponse<TenantSubscriptionDto>.SuccessResponse(subscription));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching subscription");
            return StatusCode(500, ApiResponse<TenantSubscriptionDto>.ErrorResponse("Failed to fetch subscription"));
        }
    }
}
