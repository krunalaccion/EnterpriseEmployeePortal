using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.Entities;

public class LeaveAllocation : BaseEntity
{
    public int EmployeeId { get; set; }
    public int CalendarYear { get; set; } = DateTime.UtcNow.Year;
    public LeaveType LeaveType { get; set; } = LeaveType.AnnualVacation;
    public decimal AllocatedDays { get; set; } = 20m;
    public decimal UsedDays { get; set; }

    public Employee? Employee { get; set; }
}
