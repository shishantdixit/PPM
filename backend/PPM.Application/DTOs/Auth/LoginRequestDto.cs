using System.ComponentModel.DataAnnotations;

namespace PPM.Application.DTOs.Auth;

/// <summary>
/// Login request DTO
/// </summary>
public class LoginRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Tenant code (optional for super admin login)
    /// </summary>
    public string? TenantCode { get; set; }
}
