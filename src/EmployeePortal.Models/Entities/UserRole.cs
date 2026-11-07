namespace EmployeePortal.Models.Entities;

public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
    public Role? Role { get; set; }
}

