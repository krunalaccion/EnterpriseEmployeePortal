namespace EmployeePortal.Models.DTOs;

public class DashboardSummaryDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public int PendingLeaveRequests { get; set; }
    public decimal MonthlyPayrollTotal { get; set; }
    public int OpenSecurityAlerts { get; set; }
    public List<ChartDataPointDto> HeadcountByDepartment { get; set; } = new();
    public List<ChartDataPointDto> PayrollByDepartment { get; set; } = new();
    public List<AuditLogDto> RecentAuditEvents { get; set; } = new();
}

public class ChartDataPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Category { get; set; } = string.Empty;
}

