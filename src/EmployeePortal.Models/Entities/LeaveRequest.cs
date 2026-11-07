using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.Entities;

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public LeaveType LeaveType { get; set; } = LeaveType.AnnualVacation;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveApprovalStatus Status { get; set; } = LeaveApprovalStatus.Pending;
    public int? ApprovedByUserId { get; set; }
    public string ReviewerComments { get; set; } = string.Empty;
    public DateTime? ReviewedAtUtc { get; set; }

    public Employee? Employee { get; set; }
}

