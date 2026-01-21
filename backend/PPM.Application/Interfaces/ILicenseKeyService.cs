using PPM.Application.DTOs.License;
using PPM.Core.Entities;

namespace PPM.Application.Interfaces;

/// <summary>
/// Service for managing license keys
/// </summary>
public interface ILicenseKeyService
{
    /// <summary>
    /// Generate a unique license key in format PPM-XXXX-XXXX-XXXX-XXXX
    /// </summary>
    string GenerateUniqueKey();

    /// <summary>
    /// Validate the format of a license key
    /// </summary>
    bool ValidateKeyFormat(string key);

    /// <summary>
    /// Get license key by its key string
    /// </summary>
    Task<LicenseKey?> GetByKeyAsync(string key);

    /// <summary>
    /// Get license key by ID
    /// </summary>
    Task<LicenseKey?> GetByIdAsync(Guid licenseKeyId);

    /// <summary>
    /// Create a new license key
    /// </summary>
    Task<LicenseKey> CreateLicenseKeyAsync(GenerateLicenseKeyDto dto, Guid generatedBySystemUserId);

    /// <summary>
    /// Create multiple license keys
    /// </summary>
    Task<List<LicenseKey>> CreateBatchLicenseKeysAsync(BatchGenerateLicenseKeyDto dto, Guid generatedBySystemUserId);

    /// <summary>
    /// Activate a license key for a tenant
    /// </summary>
    Task<ActivationResultDto> ActivateKeyAsync(Guid tenantId, string key);

    /// <summary>
    /// Revoke a license key
    /// </summary>
    Task<bool> RevokeKeyAsync(Guid licenseKeyId, string? reason);

    /// <summary>
    /// Get all license keys with filtering
    /// </summary>
    Task<(List<LicenseKeyDto> Keys, int TotalCount)> GetLicenseKeysAsync(LicenseKeyQueryDto query);

    /// <summary>
    /// Get subscription status for a tenant
    /// </summary>
    Task<SubscriptionStatusDto?> GetSubscriptionStatusAsync(Guid tenantId);
}
