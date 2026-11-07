using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.Entities;

public class Employee : BaseEntity
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
    public DateTime HireDate { get; set; } = DateTime.UtcNow.Date;
    public decimal BaseSalary { get; set; }
    public string EncryptedTaxId { get; set; } = string.Empty;
    public string BankRoutingNumber { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string HrNotesHtml { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int? ManagerId { get; set; }

    public Department? Department { get; set; }
    public Employee? Manager { get; set; }
    public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();
    public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveAllocation> LeaveAllocations { get; set; } = new List<LeaveAllocation>();
    public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
}

