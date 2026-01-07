using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PPM.API.Attributes;
using PPM.API.Middleware;
using PPM.Application.Common;
using PPM.Core.Entities;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[RequireTenant]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ApplicationDbContext dbContext, ILogger<UsersController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Get all users for the tenant
    /// </summary>
    [HttpGet]
    [RequireManager]
    public async Task<IActionResult> GetAll([FromQuery] string? role = null, [FromQuery] bool? isActive = null)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var query = _dbContext.Users.Where(u => u.TenantId == tenantId.Value);

            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            var users = await query
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Phone = u.Phone,
                    FullName = u.FullName,
                    Role = u.Role,
                    EmployeeCode = u.EmployeeCode,
                    DateOfJoining = u.DateOfJoining,
                    Salary = u.Salary,
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<UserDto>>.SuccessResponse(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to fetch users"));
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequireManager]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Tenant context not found"));

            var user = await _dbContext.Users
                .Where(u => u.UserId == id && u.TenantId == tenantId.Value)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Phone = u.Phone,
                    FullName = u.FullName,
                    Role = u.Role,
                    EmployeeCode = u.EmployeeCode,
                    DateOfJoining = u.DateOfJoining,
                    Salary = u.Salary,
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Failed to fetch user"));
        }
    }

    /// <summary>
    /// Get user profile with shift statistics
    /// </summary>
    [HttpGet("{id}/profile")]
    [RequireManager]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<UserProfileDto>.ErrorResponse("Tenant context not found"));

            var user = await _dbContext.Users
                .Where(u => u.UserId == id && u.TenantId == tenantId.Value)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User not found"));

            // Get shift statistics
            var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var shifts = await _dbContext.Shifts
                .Where(s => s.WorkerId == id && s.TenantId == tenantId.Value)
                .ToListAsync();

            var recentShifts = shifts.Where(s => s.ShiftDate >= thirtyDaysAgo).ToList();

            var profile = new UserProfileDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                Role = user.Role,
                EmployeeCode = user.EmployeeCode,
                DateOfJoining = user.DateOfJoining,
                Salary = user.Salary,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                // Statistics
                TotalShifts = shifts.Count,
                ShiftsLast30Days = recentShifts.Count,
                TotalSales = shifts.Where(s => s.Status == ShiftStatus.Closed).Sum(s => s.TotalSales),
                SalesLast30Days = recentShifts.Where(s => s.Status == ShiftStatus.Closed).Sum(s => s.TotalSales),
                AverageVariance = shifts.Where(s => s.Status == ShiftStatus.Closed).Any()
                    ? shifts.Where(s => s.Status == ShiftStatus.Closed).Average(s => s.Variance)
                    : 0
            };

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile {UserId}", id);
            return StatusCode(500, ApiResponse<UserProfileDto>.ErrorResponse("Failed to fetch user profile"));
        }
    }

    /// <summary>
    /// Create a new user (Worker/Manager)
    /// </summary>
    [HttpPost]
    [RequireOwner]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Tenant context not found"));

            // Validate role
            if (dto.Role != "Worker" && dto.Role != "Manager")
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Invalid role. Must be 'Worker' or 'Manager'"));

            // Check if username already exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username && u.TenantId == tenantId.Value);

            if (existingUser != null)
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Username already exists"));

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                TenantId = tenantId.Value,
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = passwordHash,
                FullName = dto.FullName,
                Role = dto.Role,
                EmployeeCode = dto.EmployeeCode,
                DateOfJoining = dto.DateOfJoining,
                Salary = dto.Salary,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created user {Username} with role {Role}", user.Username, user.Role);

            var result = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                Role = user.Role,
                EmployeeCode = user.EmployeeCode,
                DateOfJoining = user.DateOfJoining,
                Salary = user.Salary,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = user.UserId },
                ApiResponse<UserDto>.SuccessResponse(result, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Failed to create user"));
        }
    }

    /// <summary>
    /// Update user details
    /// </summary>
    [HttpPut("{id}")]
    [RequireOwner]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Tenant context not found"));

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == id && u.TenantId == tenantId.Value);

            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

            // Don't allow changing Owner's role
            if (user.Role == "Owner")
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Cannot modify Owner account"));

            // Check username uniqueness if changed
            if (dto.Username != user.Username)
            {
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Username == dto.Username && u.TenantId == tenantId.Value);
                if (existingUser != null)
                    return BadRequest(ApiResponse<UserDto>.ErrorResponse("Username already exists"));
            }

            // Update fields
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.FullName = dto.FullName;
            user.Role = dto.Role;
            user.EmployeeCode = dto.EmployeeCode;
            user.DateOfJoining = dto.DateOfJoining;
            user.Salary = dto.Salary;
            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated user {UserId}", id);

            var result = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                Role = user.Role,
                EmployeeCode = user.EmployeeCode,
                DateOfJoining = user.DateOfJoining,
                Salary = user.Salary,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(result, "User updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Failed to update user"));
        }
    }

    /// <summary>
    /// Reset user password
    /// </summary>
    [HttpPut("{id}/reset-password")]
    [RequireOwner]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == id && u.TenantId == tenantId.Value);

            if (user == null)
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
                return BadRequest(ApiResponse<object>.ErrorResponse("Password must be at least 6 characters"));

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Reset password for user {UserId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Password reset successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to reset password"));
        }
    }

    /// <summary>
    /// Activate/Deactivate user
    /// </summary>
    [HttpPut("{id}/status")]
    [RequireOwner]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == id && u.TenantId == tenantId.Value);

            if (user == null)
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

            // Don't allow deactivating Owner
            if (user.Role == "Owner")
                return BadRequest(ApiResponse<object>.ErrorResponse("Cannot deactivate Owner account"));

            // Check if user has active shifts
            if (!dto.IsActive)
            {
                var hasActiveShifts = await _dbContext.Shifts
                    .AnyAsync(s => s.WorkerId == id && s.Status == ShiftStatus.Active);
                if (hasActiveShifts)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Cannot deactivate user with active shifts"));
            }

            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            var status = dto.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("User {UserId} {Status}", id, status);

            return Ok(ApiResponse<object>.SuccessResponse(null, $"User {status} successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for user {UserId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to update user status"));
        }
    }

    /// <summary>
    /// Delete user (soft delete by deactivating)
    /// </summary>
    [HttpDelete("{id}")]
    [RequireOwner]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tenantId = HttpContext.GetTenantId();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponse<object>.ErrorResponse("Tenant context not found"));

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == id && u.TenantId == tenantId.Value);

            if (user == null)
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));

            // Don't allow deleting Owner
            if (user.Role == "Owner")
                return BadRequest(ApiResponse<object>.ErrorResponse("Cannot delete Owner account"));

            // Check if user has any shifts
            var hasShifts = await _dbContext.Shifts.AnyAsync(s => s.WorkerId == id);
            if (hasShifts)
            {
                // Soft delete - just deactivate
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Soft deleted (deactivated) user {UserId}", id);
                return Ok(ApiResponse<object>.SuccessResponse(null, "User deactivated (has shift history)"));
            }

            // Hard delete if no shifts
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted user {UserId}", id);

            return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete user"));
        }
    }
}

// DTOs
public class UserDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public decimal? Salary { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserProfileDto : UserDto
{
    public int TotalShifts { get; set; }
    public int ShiftsLast30Days { get; set; }
    public decimal TotalSales { get; set; }
    public decimal SalesLast30Days { get; set; }
    public decimal AverageVariance { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Worker or Manager
    public string? EmployeeCode { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public decimal? Salary { get; set; }
}

public class UpdateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public DateTime? DateOfJoining { get; set; }
    public decimal? Salary { get; set; }
}

public class ResetPasswordDto
{
    public string NewPassword { get; set; } = string.Empty;
}

public class UpdateUserStatusDto
{
    public bool IsActive { get; set; }
}