namespace PPM.Application.Common;

/// <summary>
/// Represents the current tenant context for the request
/// </summary>
public class TenantContext
{
    public Guid? TenantId { get; set; }
    public string? TenantCode { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsSuperAdmin { get; set; }
}
