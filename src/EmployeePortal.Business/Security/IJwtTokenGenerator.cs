using System.Security.Claims;
using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Security;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(UserSessionDto userSession);
    ClaimsPrincipal? ValidateToken(string token);
}

