using System.ComponentModel.DataAnnotations;

namespace PPM.Application.DTOs.Registration;

/// <summary>
/// Public registration request - anyone can register
/// </summary>
public class PublicRegistrationDto
{
    // Company info
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 200 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Owner name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Owner name must be between 2 and 100 characters")]
    public string OwnerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 20 characters")]
    public string Phone { get; set; } = string.Empty;

    // Tenant code - optional, auto-generated if not provided
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tenant code must be between 3 and 50 characters")]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Tenant code can only contain letters and numbers")]
    public string? TenantCode { get; set; }

    // Address (optional)
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? Address { get; set; }

    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
    public string? State { get; set; }

    [StringLength(10, ErrorMessage = "Pin code cannot exceed 10 characters")]
    public string? PinCode { get; set; }

    // Owner credentials
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Registration response with tenant and trial info
/// </summary>
public class RegistrationResponseDto
{
    public Guid TenantId { get; set; }
    public string TenantCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime TrialStartDate { get; set; }
    public DateTime TrialEndDate { get; set; }
    public int TrialDaysRemaining { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Check availability response
/// </summary>
public class AvailabilityCheckDto
{
    public bool IsAvailable { get; set; }
    public string? Message { get; set; }
    public string? SuggestedValue { get; set; }
}
