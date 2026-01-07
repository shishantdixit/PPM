using PPM.Application.DTOs.Auth;

namespace PPM.Application.Interfaces;

/// <summary>
/// JWT token generation service interface
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token
    /// </summary>
    string GenerateAccessToken(TokenClaimsDto claims);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate and get claims from token
    /// </summary>
    TokenClaimsDto? ValidateToken(string token);
}
