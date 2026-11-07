using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Web.Services;

public class PortalUserSessionService
{
    public string CurrentUsername { get; private set; } = "admin";
    public string CurrentRole { get; private set; } = "Admin";
    public int? CurrentEmployeeId { get; private set; } = 1;
    public string? JwtToken { get; private set; }

    public event Action? OnSessionChanged;

    public void SetSession(AuthResponseDto authResponse)
    {
        if (authResponse.Succeeded && authResponse.User != null)
        {
            CurrentUsername = authResponse.User.Username;
            CurrentRole = authResponse.User.Roles.FirstOrDefault() ?? "Employee";
            CurrentEmployeeId = authResponse.User.EmployeeId;
            JwtToken = authResponse.Token;
            OnSessionChanged?.Invoke();
        }
    }

    public void SwitchActiveRoleForTesting(string roleName, string username, int employeeId)
    {
        CurrentRole = roleName;
        CurrentUsername = username;
        CurrentEmployeeId = employeeId;
        OnSessionChanged?.Invoke();
    }

    public bool IsInRole(params string[] allowedRoles)
    {
        return allowedRoles.Any(r => string.Equals(r, CurrentRole, StringComparison.OrdinalIgnoreCase));
    }
}

