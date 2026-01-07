using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PPM.API.Middleware;
using PPM.Application.Common;

namespace PPM.API.Attributes;

/// <summary>
/// Authorization attribute that requires super admin access
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireSuperAdminAttribute : Attribute, IAuthorizationFilter
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

        // Check if user is super admin
        if (!tenantContext.IsSuperAdmin)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                Message = "Super admin access required",
                Timestamp = DateTime.UtcNow
            })
            {
                StatusCode = 403
            };
            return;
        }
    }
}
