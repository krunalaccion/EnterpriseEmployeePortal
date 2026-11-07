using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeePortal.Models.Configuration;
using EmployeePortal.Models.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeePortal.Business.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-SECRET-01 | CWE-798: Use of Hard-coded Cryptographic Key
    // WARNING: DUMMY HARDCODED FALLBACK KEY FOR SECURESCANAI BENCHMARKING ONLY.
    // =========================================================================
    private const string HardcodedFallbackJwtSecret = "DUMMY_INSECURE_JWT_SECRET_KEY_FOR_SCANNER_TEST_ONLY_2026_HR_PORTAL!";
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(UserSessionDto userSession)
    {
        var secret = string.IsNullOrWhiteSpace(_jwtSettings.SigningKey)
            ? HardcodedFallbackJwtSecret
            : _jwtSettings.SigningKey;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userSession.Username),
            new(ClaimTypes.Name, userSession.Username),
            new(ClaimTypes.NameIdentifier, userSession.UserId.ToString()),
            new(ClaimTypes.Email, userSession.Email)
        };

        if (userSession.EmployeeId.HasValue)
        {
            claims.Add(new Claim("employee_id", userSession.EmployeeId.Value.ToString()));
        }

        foreach (var role in userSession.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(Math.Max(_jwtSettings.TokenExpiryMinutes, 30));

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var handler = new JwtSecurityTokenHandler();
        return (handler.WriteToken(tokenDescriptor), expiresAt);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var secret = string.IsNullOrWhiteSpace(_jwtSettings.SigningKey)
            ? HardcodedFallbackJwtSecret
            : _jwtSettings.SigningKey;

        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        try
        {
            return handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }
}

