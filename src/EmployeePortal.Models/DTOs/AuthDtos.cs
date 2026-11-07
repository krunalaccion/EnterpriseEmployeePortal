using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.Models.DTOs;

public class LoginRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public UserSessionDto? User { get; set; }
}

public class UserSessionDto
{
    public int UserId { get; set; }
    public int? EmployeeId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

