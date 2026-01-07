using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PPM.Application.Common;
using PPM.Application.DTOs.Auth;
using PPM.Application.Interfaces;

namespace PPM.Infrastructure.Services;

/// <summary>
/// JWT token generation and validation service
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateAccessToken(TokenClaimsDto claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var tokenClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, claims.UserId.ToString()),
            new Claim(ClaimTypes.Name, claims.Username),
            new Claim(ClaimTypes.Role, claims.Role),
            new Claim("IsSuperAdmin", claims.IsSuperAdmin.ToString())
        };

        // Add tenant claims if not super admin
        if (claims.TenantId.HasValue)
        {
            tokenClaims.Add(new Claim("TenantId", claims.TenantId.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(claims.TenantCode))
        {
            tokenClaims.Add(new Claim("TenantCode", claims.TenantCode));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(tokenClaims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public TokenClaimsDto? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;
            var isSuperAdmin = bool.Parse(jwtToken.Claims.First(x => x.Type == "IsSuperAdmin").Value);

            Guid? tenantId = null;
            string? tenantCode = null;

            var tenantIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "TenantId");
            if (tenantIdClaim != null)
            {
                tenantId = Guid.Parse(tenantIdClaim.Value);
            }

            var tenantCodeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "TenantCode");
            if (tenantCodeClaim != null)
            {
                tenantCode = tenantCodeClaim.Value;
            }

            return new TokenClaimsDto
            {
                UserId = userId,
                Username = username,
                Role = role,
                TenantId = tenantId,
                TenantCode = tenantCode,
                IsSuperAdmin = isSuperAdmin
            };
        }
        catch
        {
            return null;
        }
    }
}
