using System.Security.Claims;
using PPM.Application.Common;

namespace PPM.API.Middleware;

/// <summary>
/// Middleware to resolve tenant context from JWT claims
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantContext = new TenantContext
            {
                UserId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                Username = context.User.FindFirstValue(ClaimTypes.Name)!,
                Role = context.User.FindFirstValue(ClaimTypes.Role)!,
                IsSuperAdmin = bool.Parse(context.User.FindFirstValue("IsSuperAdmin") ?? "false")
            };

            // Extract tenant claims if present (not present for super admin)
            var tenantIdClaim = context.User.FindFirstValue("TenantId");
            if (!string.IsNullOrEmpty(tenantIdClaim))
            {
                tenantContext.TenantId = Guid.Parse(tenantIdClaim);
                tenantContext.TenantCode = context.User.FindFirstValue("TenantCode");
            }

            // Store in HttpContext.Items for easy access
            context.Items["TenantContext"] = tenantContext;
        }

        await _next(context);
    }
}

/// <summary>
/// Extension methods for accessing tenant context
/// </summary>
public static class TenantContextExtensions
{
    public const string TenantContextKey = "TenantContext";

    public static TenantContext? GetTenantContext(this HttpContext context)
    {
        return context.Items[TenantContextKey] as TenantContext;
    }

    public static Guid? GetTenantId(this HttpContext context)
    {
        return context.GetTenantContext()?.TenantId;
    }

    public static string? GetTenantCode(this HttpContext context)
    {
        return context.GetTenantContext()?.TenantCode;
    }

    public static Guid GetUserId(this HttpContext context)
    {
        return context.GetTenantContext()?.UserId ?? Guid.Empty;
    }

    public static bool IsSuperAdmin(this HttpContext context)
    {
        return context.GetTenantContext()?.IsSuperAdmin ?? false;
    }
}
