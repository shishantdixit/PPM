using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Super admin users who manage the entire SaaS platform
/// </summary>
public class SystemUser : BaseEntity
{
    public Guid SystemUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "SuperAdmin";
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}
