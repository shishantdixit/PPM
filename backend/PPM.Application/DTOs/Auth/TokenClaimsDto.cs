namespace PPM.Application.DTOs.Auth;

/// <summary>
/// Claims to be included in JWT token
/// </summary>
public class TokenClaimsDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public string? TenantCode { get; set; }
    public bool IsSuperAdmin { get; set; }
}
