using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PPM.API.Middleware;
using PPM.Application.Common;

namespace PPM.API.Attributes;

/// <summary>
/// Authorization attribute that requires user to have one of the specified roles
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;
    private readonly bool _allowSuperAdmin;

    public RequireRoleAttribute(params string[] roles)
    {
        _roles = roles;
        _allowSuperAdmin = true; // Super admin can access by default
    }

    public RequireRoleAttribute(bool allowSuperAdmin, params string[] roles)
    {
        _roles = roles;
        _allowSuperAdmin = allowSuperAdmin;
    }

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

        // Allow super admin if permitted
        if (_allowSuperAdmin && tenantContext.IsSuperAdmin)
        {
            return;
        }

        // Check if user has one of the required roles
        if (!_roles.Contains(tenantContext.Role))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}

/// <summary>
/// Requires Owner role
/// </summary>
public class RequireOwnerAttribute : RequireRoleAttribute
{
    public RequireOwnerAttribute() : base("Owner") { }
}

/// <summary>
/// Requires Manager role (or higher: Owner, SuperAdmin)
/// </summary>
public class RequireManagerAttribute : RequireRoleAttribute
{
    public RequireManagerAttribute() : base("Manager", "Owner") { }
}

/// <summary>
/// Requires Worker role (or higher: Manager, Owner, SuperAdmin)
/// </summary>
public class RequireWorkerAttribute : RequireRoleAttribute
{
    public RequireWorkerAttribute() : base("Worker", "Manager", "Owner") { }
}
