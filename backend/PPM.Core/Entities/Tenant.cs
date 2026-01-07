using PPM.Core.Common;

namespace PPM.Core.Entities;

/// <summary>
/// Represents a petrol pump client in the multi-tenant system
/// </summary>
public class Tenant : BaseEntity
{
    public Guid TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string Country { get; set; } = "India";
    public string? PinCode { get; set; }

    // Subscription
    public string SubscriptionPlan { get; set; } = "Basic"; // Basic, Premium, Enterprise
    public string SubscriptionStatus { get; set; } = "Active"; // Active, Suspended, Expired
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }

    // Limits
    public int MaxMachines { get; set; } = 5;
    public int MaxWorkers { get; set; } = 20;
    public int MaxMonthlyBills { get; set; } = 10000;

    // System
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
