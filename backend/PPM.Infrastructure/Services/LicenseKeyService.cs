using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PPM.Application.DTOs.License;
using PPM.Application.Interfaces;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.Infrastructure.Services;

/// <summary>
/// Service for managing license keys
/// </summary>
public class LicenseKeyService : ILicenseKeyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<LicenseKeyService> _logger;

    // Key format: PPM-XXXX-XXXX-XXXX-XXXX
    private const string KeyPrefix = "PPM";
    private const int SegmentLength = 4;
    private const int SegmentCount = 4;
    // Exclude confusing characters: 0/O, 1/I/L
    private const string AllowedChars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    // Default limits by plan
    private static readonly Dictionary<string, (int Machines, int Workers, int Bills)> PlanDefaults = new()
    {
        ["Basic"] = (5, 20, 10000),
        ["Premium"] = (15, 50, 50000),
        ["Enterprise"] = (50, 200, 200000)
    };

    public LicenseKeyService(ApplicationDbContext dbContext, ILogger<LicenseKeyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public string GenerateUniqueKey()
    {
        var segments = new List<string> { KeyPrefix };

        using var rng = RandomNumberGenerator.Create();
        for (int s = 0; s < SegmentCount; s++)
        {
            var segment = new char[SegmentLength];
            var bytes = new byte[SegmentLength];
            rng.GetBytes(bytes);
            for (int i = 0; i < SegmentLength; i++)
            {
                segment[i] = AllowedChars[bytes[i] % AllowedChars.Length];
            }
            segments.Add(new string(segment));
        }

        return string.Join("-", segments);
    }

    public bool ValidateKeyFormat(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        var pattern = @"^PPM-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$";
        return Regex.IsMatch(key.ToUpper().Trim(), pattern);
    }

    public async Task<LicenseKey?> GetByKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        return await _dbContext.LicenseKeys
            .Include(lk => lk.ActivatedByTenant)
            .Include(lk => lk.GeneratedBySystemUser)
            .FirstOrDefaultAsync(lk => lk.Key == key.ToUpper().Trim());
    }

    public async Task<LicenseKey?> GetByIdAsync(Guid licenseKeyId)
    {
        return await _dbContext.LicenseKeys
            .Include(lk => lk.ActivatedByTenant)
            .Include(lk => lk.GeneratedBySystemUser)
            .FirstOrDefaultAsync(lk => lk.LicenseKeyId == licenseKeyId);
    }

    public async Task<LicenseKey> CreateLicenseKeyAsync(GenerateLicenseKeyDto dto, Guid generatedBySystemUserId)
    {
        var defaults = PlanDefaults.GetValueOrDefault(dto.SubscriptionPlan, PlanDefaults["Basic"]);

        // Generate unique key
        string key;
        do
        {
            key = GenerateUniqueKey();
        } while (await _dbContext.LicenseKeys.AnyAsync(lk => lk.Key == key));

        var licenseKey = new LicenseKey
        {
            LicenseKeyId = Guid.NewGuid(),
            Key = key,
            SubscriptionPlan = dto.SubscriptionPlan,
            DurationMonths = dto.DurationMonths,
            MaxMachines = dto.MaxMachines ?? defaults.Machines,
            MaxWorkers = dto.MaxWorkers ?? defaults.Workers,
            MaxMonthlyBills = dto.MaxMonthlyBills ?? defaults.Bills,
            Status = "Available",
            GeneratedBySystemUserId = generatedBySystemUserId,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.LicenseKeys.Add(licenseKey);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Generated license key {Key} for plan {Plan} by {UserId}",
            key, dto.SubscriptionPlan, generatedBySystemUserId);

        return licenseKey;
    }

    public async Task<List<LicenseKey>> CreateBatchLicenseKeysAsync(BatchGenerateLicenseKeyDto dto, Guid generatedBySystemUserId)
    {
        var keys = new List<LicenseKey>();
        var defaults = PlanDefaults.GetValueOrDefault(dto.SubscriptionPlan, PlanDefaults["Basic"]);

        for (int i = 0; i < dto.Count; i++)
        {
            string key;
            do
            {
                key = GenerateUniqueKey();
            } while (await _dbContext.LicenseKeys.AnyAsync(lk => lk.Key == key) || keys.Any(k => k.Key == key));

            var licenseKey = new LicenseKey
            {
                LicenseKeyId = Guid.NewGuid(),
                Key = key,
                SubscriptionPlan = dto.SubscriptionPlan,
                DurationMonths = dto.DurationMonths,
                MaxMachines = dto.MaxMachines ?? defaults.Machines,
                MaxWorkers = dto.MaxWorkers ?? defaults.Workers,
                MaxMonthlyBills = dto.MaxMonthlyBills ?? defaults.Bills,
                Status = "Available",
                GeneratedBySystemUserId = generatedBySystemUserId,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            keys.Add(licenseKey);
        }

        _dbContext.LicenseKeys.AddRange(keys);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Generated {Count} license keys for plan {Plan} by {UserId}",
            dto.Count, dto.SubscriptionPlan, generatedBySystemUserId);

        return keys;
    }

    public async Task<ActivationResultDto> ActivateKeyAsync(Guid tenantId, string key)
    {
        var normalizedKey = key.ToUpper().Trim();

        if (!ValidateKeyFormat(normalizedKey))
        {
            return new ActivationResultDto
            {
                Success = false,
                Message = "Invalid license key format. Expected format: PPM-XXXX-XXXX-XXXX-XXXX"
            };
        }

        var licenseKey = await GetByKeyAsync(normalizedKey);

        if (licenseKey == null)
        {
            return new ActivationResultDto
            {
                Success = false,
                Message = "License key not found"
            };
        }

        if (licenseKey.Status != "Available")
        {
            var statusMessage = licenseKey.Status switch
            {
                "Used" => "This license key has already been used",
                "Revoked" => "This license key has been revoked",
                _ => "This license key is not available for activation"
            };

            return new ActivationResultDto
            {
                Success = false,
                Message = statusMessage
            };
        }

        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            return new ActivationResultDto
            {
                Success = false,
                Message = "Tenant not found"
            };
        }

        // Update license key
        licenseKey.Status = "Used";
        licenseKey.ActivatedByTenantId = tenantId;
        licenseKey.ActivatedAt = DateTime.UtcNow;
        licenseKey.UpdatedAt = DateTime.UtcNow;

        // Update tenant subscription
        var now = DateTime.UtcNow;
        tenant.IsTrial = false;
        tenant.SubscriptionStatus = "Active";
        tenant.SubscriptionPlan = licenseKey.SubscriptionPlan;
        tenant.SubscriptionStartDate = now;
        tenant.SubscriptionEndDate = now.AddMonths(licenseKey.DurationMonths);
        tenant.MaxMachines = licenseKey.MaxMachines;
        tenant.MaxWorkers = licenseKey.MaxWorkers;
        tenant.MaxMonthlyBills = licenseKey.MaxMonthlyBills;
        tenant.ActiveLicenseKeyId = licenseKey.LicenseKeyId;
        tenant.LicenseActivatedAt = now;
        tenant.UpdatedAt = now;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("License key {Key} activated by tenant {TenantId}", normalizedKey, tenantId);

        return new ActivationResultDto
        {
            Success = true,
            Message = $"License key activated successfully. Your {licenseKey.SubscriptionPlan} subscription is now active.",
            NewSubscriptionPlan = licenseKey.SubscriptionPlan,
            NewSubscriptionEndDate = tenant.SubscriptionEndDate,
            MaxMachines = licenseKey.MaxMachines,
            MaxWorkers = licenseKey.MaxWorkers,
            MaxMonthlyBills = licenseKey.MaxMonthlyBills
        };
    }

    public async Task<bool> RevokeKeyAsync(Guid licenseKeyId, string? reason)
    {
        var licenseKey = await _dbContext.LicenseKeys.FindAsync(licenseKeyId);

        if (licenseKey == null)
        {
            return false;
        }

        if (licenseKey.Status == "Used")
        {
            _logger.LogWarning("Cannot revoke license key {KeyId} - already used by tenant {TenantId}",
                licenseKeyId, licenseKey.ActivatedByTenantId);
            return false;
        }

        licenseKey.Status = "Revoked";
        if (!string.IsNullOrWhiteSpace(reason))
        {
            licenseKey.Notes = $"{licenseKey.Notes}\n[Revoked] {reason}".Trim();
        }
        licenseKey.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("License key {KeyId} revoked. Reason: {Reason}", licenseKeyId, reason ?? "Not specified");

        return true;
    }

    public async Task<(List<LicenseKeyDto> Keys, int TotalCount)> GetLicenseKeysAsync(LicenseKeyQueryDto query)
    {
        var queryable = _dbContext.LicenseKeys
            .Include(lk => lk.ActivatedByTenant)
            .Include(lk => lk.GeneratedBySystemUser)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            queryable = queryable.Where(lk => lk.Status == query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.SubscriptionPlan))
        {
            queryable = queryable.Where(lk => lk.SubscriptionPlan == query.SubscriptionPlan);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToUpper();
            queryable = queryable.Where(lk =>
                lk.Key.Contains(search) ||
                (lk.ActivatedByTenant != null && lk.ActivatedByTenant.TenantCode.Contains(search)) ||
                (lk.ActivatedByTenant != null && lk.ActivatedByTenant.CompanyName.Contains(search)));
        }

        var totalCount = await queryable.CountAsync();

        var keys = await queryable
            .OrderByDescending(lk => lk.CreatedAt)
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .Select(lk => new LicenseKeyDto
            {
                LicenseKeyId = lk.LicenseKeyId,
                Key = lk.Key,
                SubscriptionPlan = lk.SubscriptionPlan,
                DurationMonths = lk.DurationMonths,
                MaxMachines = lk.MaxMachines,
                MaxWorkers = lk.MaxWorkers,
                MaxMonthlyBills = lk.MaxMonthlyBills,
                Status = lk.Status,
                ActivatedByTenantId = lk.ActivatedByTenantId,
                ActivatedByTenantCode = lk.ActivatedByTenant != null ? lk.ActivatedByTenant.TenantCode : null,
                ActivatedByCompanyName = lk.ActivatedByTenant != null ? lk.ActivatedByTenant.CompanyName : null,
                ActivatedAt = lk.ActivatedAt,
                GeneratedByUsername = lk.GeneratedBySystemUser.Username,
                Notes = lk.Notes,
                CreatedAt = lk.CreatedAt
            })
            .ToListAsync();

        return (keys, totalCount);
    }

    public async Task<SubscriptionStatusDto?> GetSubscriptionStatusAsync(Guid tenantId)
    {
        var tenant = await _dbContext.Tenants.FindAsync(tenantId);
        if (tenant == null)
        {
            return null;
        }

        // Get current usage counts
        var machineCount = await _dbContext.Machines.CountAsync(m => m.TenantId == tenantId && m.IsActive);
        var workerCount = await _dbContext.Users.CountAsync(u => u.TenantId == tenantId && u.Role == "Worker" && u.IsActive);

        // Get current month's bill count
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var billCount = await _dbContext.FuelSales.CountAsync(fs => fs.TenantId == tenantId && fs.SaleTime >= startOfMonth);

        int? trialDaysRemaining = null;
        int? subscriptionDaysRemaining = null;

        if (tenant.IsTrial && tenant.TrialEndDate.HasValue)
        {
            trialDaysRemaining = Math.Max(0, (int)(tenant.TrialEndDate.Value - DateTime.UtcNow).TotalDays);
        }

        if (tenant.SubscriptionEndDate.HasValue)
        {
            subscriptionDaysRemaining = Math.Max(0, (int)(tenant.SubscriptionEndDate.Value - DateTime.UtcNow).TotalDays);
        }

        return new SubscriptionStatusDto
        {
            TenantId = tenant.TenantId,
            TenantCode = tenant.TenantCode,
            CompanyName = tenant.CompanyName,
            IsTrial = tenant.IsTrial,
            TrialStartDate = tenant.TrialStartDate,
            TrialEndDate = tenant.TrialEndDate,
            TrialDaysRemaining = trialDaysRemaining,
            SubscriptionPlan = tenant.SubscriptionPlan,
            SubscriptionStatus = tenant.SubscriptionStatus,
            SubscriptionStartDate = tenant.SubscriptionStartDate,
            SubscriptionEndDate = tenant.SubscriptionEndDate,
            DaysRemaining = subscriptionDaysRemaining,
            MaxMachines = tenant.MaxMachines,
            MaxWorkers = tenant.MaxWorkers,
            MaxMonthlyBills = tenant.MaxMonthlyBills,
            CurrentMachineCount = machineCount,
            CurrentWorkerCount = workerCount,
            CurrentMonthBillCount = billCount,
            ActiveLicenseKeyId = tenant.ActiveLicenseKeyId,
            LicenseActivatedAt = tenant.LicenseActivatedAt
        };
    }
}
