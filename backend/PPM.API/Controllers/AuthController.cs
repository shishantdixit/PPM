using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PPM.Application.Common;
using PPM.Application.DTOs.Auth;
using PPM.Application.Interfaces;
using PPM.Infrastructure.Data;

namespace PPM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthController> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint for all user types
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Check if this is a super admin login (no tenant code)
            if (string.IsNullOrEmpty(request.TenantCode))
            {
                return await LoginSuperAdmin(request);
            }

            // Tenant user login
            return await LoginTenantUser(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResponse("An error occurred during login"));
        }
    }

    /// <summary>
    /// Super admin login
    /// </summary>
    private async Task<IActionResult> LoginSuperAdmin(LoginRequestDto request)
    {
        var systemUser = await _dbContext.SystemUsers
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (systemUser == null)
        {
            _logger.LogWarning("Failed login attempt for super admin user {Username}", request.Username);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password"));
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, systemUser.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for super admin {Username} - incorrect password", request.Username);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password"));
        }

        // Generate token
        var tokenClaims = new TokenClaimsDto
        {
            UserId = systemUser.SystemUserId,
            Username = systemUser.Username,
            Role = systemUser.Role,
            TenantId = null,
            TenantCode = null,
            IsSuperAdmin = true
        };

        var token = _tokenService.GenerateAccessToken(tokenClaims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Update last login
        systemUser.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.ExpiryMinutes * 60,
            User = new UserInfoDto
            {
                UserId = systemUser.SystemUserId,
                Username = systemUser.Username,
                FullName = systemUser.FullName,
                Email = systemUser.Email,
                Role = systemUser.Role,
                TenantId = null,
                TenantCode = null,
                TenantName = null,
                IsSuperAdmin = true
            }
        };

        _logger.LogInformation("Super admin {Username} logged in successfully", systemUser.Username);

        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful"));
    }

    /// <summary>
    /// Tenant user login (Owner, Manager, Worker)
    /// </summary>
    private async Task<IActionResult> LoginTenantUser(LoginRequestDto request)
    {
        // Find tenant
        var tenant = await _dbContext.Tenants
            .FirstOrDefaultAsync(t => t.TenantCode == request.TenantCode && t.IsActive);

        if (tenant == null)
        {
            _logger.LogWarning("Failed login attempt - tenant {TenantCode} not found", request.TenantCode);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Invalid tenant or credentials"));
        }

        // Check subscription status
        if (tenant.SubscriptionStatus != "Active")
        {
            _logger.LogWarning("Login attempt for suspended tenant {TenantCode}", request.TenantCode);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Account suspended. Please contact support."));
        }

        // Find user
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.TenantId == tenant.TenantId
                && u.Username == request.Username
                && u.IsActive);

        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for user {Username} in tenant {TenantCode}",
                request.Username, request.TenantCode);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password"));
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for user {Username} in tenant {TenantCode} - incorrect password",
                request.Username, request.TenantCode);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password"));
        }

        // Generate token
        var tokenClaims = new TokenClaimsDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role,
            TenantId = tenant.TenantId,
            TenantCode = tenant.TenantCode,
            IsSuperAdmin = false
        };

        var token = _tokenService.GenerateAccessToken(tokenClaims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.ExpiryMinutes * 60,
            User = new UserInfoDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = user.Role,
                TenantId = tenant.TenantId,
                TenantCode = tenant.TenantCode,
                TenantName = tenant.CompanyName,
                IsSuperAdmin = false
            }
        };

        _logger.LogInformation("User {Username} from tenant {TenantCode} logged in successfully",
            user.Username, tenant.TenantCode);

        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful"));
    }

    /// <summary>
    /// Logout endpoint
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // In a stateless JWT system, logout is handled client-side by removing the token
        // Future enhancement: implement token blacklist or refresh token revocation
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Logout successful"));
    }

    /// <summary>
    /// Change password endpoint
    /// </summary>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        // This will be implemented in a future update with authentication middleware
        // For now, return not implemented
        return StatusCode(501, ApiResponse<object>.ErrorResponse("Not implemented yet"));
    }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
