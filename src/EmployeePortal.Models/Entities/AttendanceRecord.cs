namespace EmployeePortal.Models.Entities;

public class AttendanceRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public DateTime ClockInUtc { get; set; }
    public DateTime? ClockOutUtc { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public string AttendanceStatus { get; set; } = "Present";
    public string SourceIpAddress { get; set; } = "127.0.0.1";
    public string ShiftNotes { get; set; } = string.Empty;

    public Employee? Employee { get; set; }
}

