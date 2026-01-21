namespace PPM.Application.DTOs.Auth;

/// <summary>
/// Login response with token and user info
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } // in seconds
    public UserInfoDto User { get; set; } = null!;
    public TrialInfoDto? TrialInfo { get; set; }
}

/// <summary>
/// User information DTO
/// </summary>
public class UserInfoDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public string? TenantCode { get; set; }
    public string? TenantName { get; set; }
    public bool IsSuperAdmin { get; set; }
}

/// <summary>
/// Trial information for tenant users
/// </summary>
public class TrialInfoDto
{
    public bool IsTrial { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public int DaysRemaining { get; set; }
    public string SubscriptionStatus { get; set; } = string.Empty;
}
