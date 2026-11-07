using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models.DTOs;

public class UserAccountDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHashAlgorithm { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsLockedOut { get; set; }
    public DateTime? LastLoginUtc { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PermissionsCsv { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public int AssignedUserCount { get; set; }
}

public class AssignUserRoleDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string RoleName { get; set; } = string.Empty;
}

