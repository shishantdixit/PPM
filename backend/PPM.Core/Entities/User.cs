using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Tenant users: Owner, Manager, Worker
/// </summary>
public class User : TenantEntity
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Owner, Manager, Worker

    // Worker specific
    public string? EmployeeCode { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public decimal? Salary { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
}
