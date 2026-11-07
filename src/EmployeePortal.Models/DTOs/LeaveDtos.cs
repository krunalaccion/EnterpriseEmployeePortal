using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.DTOs;

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveApprovalStatus Status { get; set; }
    public string ReviewerComments { get; set; } = string.Empty;
    public DateTime? ReviewedAtUtc { get; set; }
}

public class SubmitLeaveRequestDto
{
    public int EmployeeId { get; set; }
    public LeaveType LeaveType { get; set; } = LeaveType.AnnualVacation;
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date.AddDays(7);
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddDays(9);
    public string Reason { get; set; } = string.Empty;
}

public class ReviewLeaveRequestDto
{
    public int LeaveRequestId { get; set; }
    public LeaveApprovalStatus NewStatus { get; set; }
    public string ReviewerComments { get; set; } = string.Empty;
}

