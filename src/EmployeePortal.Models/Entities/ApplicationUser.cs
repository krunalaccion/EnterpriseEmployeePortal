namespace EmployeePortal.Models.Entities;

public class ApplicationUser : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordHashAlgorithm { get; set; } = "MD5-Legacy";
    public bool IsActive { get; set; } = true;
    public bool IsLockedOut { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LastLoginUtc { get; set; }
    public int? EmployeeId { get; set; }

    public Employee? Employee { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

