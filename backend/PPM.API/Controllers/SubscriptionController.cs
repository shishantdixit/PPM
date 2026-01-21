using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.DTOs.License;
using PPM.Application.Interfaces;

namespace PPM.API.Controllers;

/// <summary>
/// Subscription management controller for tenant owners
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class SubscriptionController : ControllerBase
{
    private readonly ILicenseKeyService _licenseKeyService;
    private readonly IFeatureService _featureService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        ILicenseKeyService licenseKeyService,
        IFeatureService featureService,
        ILogger<SubscriptionController> logger)
    {
        _licenseKeyService = licenseKeyService;
        _featureService = featureService;
        _logger = logger;
    }

    /// <summary>
    /// Get current subscription status
    /// </summary>
    [HttpGet("status")]
    [RequireOwner]
    public async Task<IActionResult> GetSubscriptionStatus()
    {
        var tenantId = HttpContext.GetTenantId();
        if (tenantId == null)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Tenant context not found"));
        }

        var status = await _licenseKeyService.GetSubscriptionStatusAsync(tenantId.Value);

        if (status == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Tenant not found"));
        }

        return Ok(ApiResponse<SubscriptionStatusDto>.SuccessResponse(status));
    }

    /// <summary>
    /// Activate a license key to upgrade subscription
    /// </summary>
    [HttpPost("activate")]
    [RequireOwner]
    public async Task<IActionResult> ActivateLicenseKey([FromBody] ActivateLicenseKeyDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse<object>.ErrorResponse($"Validation failed: {string.Join(", ", errors)}"));
        }

        var tenantId = HttpContext.GetTenantId();
        if (tenantId == null)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Tenant context not found"));
        }

        var tenantCode = HttpContext.GetTenantCode();
        _logger.LogInformation("License key activation attempt by tenant {TenantCode}", tenantCode);

        var result = await _licenseKeyService.ActivateKeyAsync(tenantId.Value, dto.LicenseKey);

        if (!result.Success)
        {
            _logger.LogWarning("License key activation failed for tenant {TenantCode}: {Message}",
                tenantCode, result.Message);
            return BadRequest(ApiResponse<ActivationResultDto>.ErrorResponse(result.Message));
        }

        _logger.LogInformation("License key activated successfully for tenant {TenantCode}. Plan: {Plan}",
            tenantCode, result.NewSubscriptionPlan);

        return Ok(ApiResponse<ActivationResultDto>.SuccessResponse(result, "License key activated successfully"));
    }

    /// <summary>
    /// Get detailed subscription info including features
    /// </summary>
    [HttpGet("details")]
    [RequireOwner]
    public async Task<IActionResult> GetSubscriptionDetails()
    {
        var tenantId = HttpContext.GetTenantId();
        if (tenantId == null)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Tenant context not found"));
        }

        var subscription = await _featureService.GetTenantSubscriptionAsync(tenantId.Value);

        if (subscription == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Subscription not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(subscription));
    }
}
