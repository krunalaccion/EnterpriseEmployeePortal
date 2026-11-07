namespace EmployeePortal.Models.DTOs;

public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime WorkDate { get; set; }
    public DateTime ClockInUtc { get; set; }
    public DateTime? ClockOutUtc { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public string AttendanceStatus { get; set; } = string.Empty;
    public string SourceIpAddress { get; set; } = string.Empty;
    public string ShiftNotes { get; set; } = string.Empty;
}

public class ClockInOutRequestDto
{
    public int EmployeeId { get; set; }
    public string ShiftNotes { get; set; } = string.Empty;
    public decimal OvertimeHours { get; set; }
}

