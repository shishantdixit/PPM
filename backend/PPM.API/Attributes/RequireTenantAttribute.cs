using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PPM.API.Middleware;
using PPM.Application.Common;

namespace PPM.API.Attributes;

/// <summary>
/// Authorization attribute that requires user to belong to a tenant (not super admin)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireTenantAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var tenantContext = context.HttpContext.GetTenantContext();

        // Check if user is authenticated
        if (tenantContext == null)
        {
            context.Result = new UnauthorizedObjectResult(new ApiResponse<object>
            {
                Success = false,
                Message = "Authentication required",
                Timestamp = DateTime.UtcNow
            });
            return;
        }

        // Allow super admins to access tenant resources (they can see all tenants)
        // For regular users, ensure they have a tenant context
        if (!tenantContext.IsSuperAdmin && tenantContext.TenantId == null)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                Message = "Tenant access required. This operation is only available to tenant users",
                Timestamp = DateTime.UtcNow
            })
            {
                StatusCode = 403
            };
            return;
        }
    }
}
