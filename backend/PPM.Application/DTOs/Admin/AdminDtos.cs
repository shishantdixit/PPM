namespace PPM.Application.DTOs.Admin;

public class SystemDashboardDto
{
    public int TotalTenants { get; set; }
    public int ActiveTenants { get; set; }
    public int SuspendedTenants { get; set; }
    public int ExpiredTenants { get; set; }
    public int TotalUsers { get; set; }
    public decimal TotalSalesAllTime { get; set; }
    public decimal TotalSalesThisMonth { get; set; }
    public List<TenantSummaryDto> TopTenants { get; set; } = new();
    public List<MonthlyStatsDto> MonthlyGrowth { get; set; } = new();
    public SubscriptionBreakdownDto SubscriptionBreakdown { get; set; } = new();
}

public class TenantSummaryDto
{
    public Guid TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? State { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string SubscriptionStatus { get; set; } = string.Empty;
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public int MaxMachines { get; set; }
    public int MaxWorkers { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Stats
    public int UserCount { get; set; }
    public int MachineCount { get; set; }
    public int NozzleCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal ThisMonthSales { get; set; }
    public int TotalShifts { get; set; }
}

public class TenantDetailDto : TenantSummaryDto
{
    public string? Address { get; set; }
    public string? PinCode { get; set; }
    public string Country { get; set; } = string.Empty;
    public int MaxMonthlyBills { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TenantUserDto> Users { get; set; } = new();
    public List<MonthlySalesDto> SalesHistory { get; set; } = new();
}

public class TenantUserDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class MonthlyStatsDto
{
    public string Month { get; set; } = string.Empty;
    public int NewTenants { get; set; }
    public decimal TotalSales { get; set; }
}

public class MonthlySalesDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public int ShiftCount { get; set; }
}

public class SubscriptionBreakdownDto
{
    public int Basic { get; set; }
    public int Premium { get; set; }
    public int Enterprise { get; set; }
}

public class UpdateSubscriptionDto
{
    public string SubscriptionPlan { get; set; } = string.Empty;
    public DateTime? SubscriptionEndDate { get; set; }
    public int? MaxMachines { get; set; }
    public int? MaxWorkers { get; set; }
    public int? MaxMonthlyBills { get; set; }
}

public class CreateOwnerUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
