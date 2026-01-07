using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Application.Interfaces;

namespace PPM.API.Attributes;

/// <summary>
/// Authorization attribute that checks if the current tenant has access to a specific feature.
/// Returns 403 with upgrade information if feature is not available.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireFeatureAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _featureCode;

    public RequireFeatureAttribute(string featureCode)
    {
        _featureCode = featureCode;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var tenantContext = context.HttpContext.GetTenantContext();

        // Super admin always has access to all features
        if (tenantContext?.IsSuperAdmin == true)
        {
            return;
        }

        // Check if tenant context exists
        if (tenantContext?.TenantId == null)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                Message = "Tenant context not found",
                Data = null
            })
            {
                StatusCode = 403
            };
            return;
        }

        // Get the feature service from DI
        var featureService = context.HttpContext.RequestServices.GetRequiredService<IFeatureService>();

        // Check if tenant has access to the feature
        var hasAccess = await featureService.HasFeatureAccessAsync(tenantContext.TenantId.Value, _featureCode);

        if (!hasAccess)
        {
            var featureAccess = await featureService.GetTenantFeatureAccessAsync(tenantContext.TenantId.Value);

            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                Message = "This feature requires an upgraded subscription plan",
                Data = new
                {
                    FeatureCode = _featureCode,
                    RequiresUpgrade = true,
                    CurrentPlan = featureAccess.SubscriptionPlan,
                    RequiredPlan = featureAccess.RequiredPlanForUpgrade
                }
            })
            {
                StatusCode = 403
            };
        }
    }
}
