using AutoMapper;
using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Enums;

namespace EmployeePortal.Business.Services;

public class DashboardService : IDashboardService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPayrollRepository _payrollRepository;
    private readonly ILeaveRepository _leaveRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;

    public DashboardService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IPayrollRepository payrollRepository,
        ILeaveRepository leaveRepository,
        IAuditLogRepository auditLogRepository,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _payrollRepository = payrollRepository;
        _leaveRepository = leaveRepository;
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        var departments = await _departmentRepository.GetDepartmentsWithEmployeesAsync(cancellationToken);
        var payrolls = await _payrollRepository.GetPayrollRecordsWithEmployeeAsync(null, cancellationToken);
        var leaves = await _leaveRepository.GetLeaveRequestsWithEmployeeAsync(null, cancellationToken);
        var recentAudits = await _auditLogRepository.GetRecentLogsAsync(8, null, cancellationToken);

        var summary = new DashboardSummaryDto
        {
            TotalEmployees = employees.Count,
            ActiveEmployees = employees.Count(e => e.Status == EmploymentStatus.Active),
            TotalDepartments = departments.Count,
            PendingLeaveRequests = leaves.Count(l => l.Status == LeaveApprovalStatus.Pending),
            MonthlyPayrollTotal = payrolls.Sum(p => p.NetPay),
            OpenSecurityAlerts = recentAudits.Count(a => a.Severity >= AuditSeverity.SecurityAlert),
            HeadcountByDepartment = departments
                .Select(d => new ChartDataPointDto
                {
                    Label = d.Name,
                    Value = d.Employees.Count,
                    Category = d.Code
                })
                .OrderByDescending(c => c.Value)
                .ToList(),
            PayrollByDepartment = departments
                .Select(d => new ChartDataPointDto
                {
                    Label = d.Name,
                    Value = d.Employees.Sum(e => e.BaseSalary / 12m),
                    Category = d.CostCenter
                })
                .OrderByDescending(c => c.Value)
                .ToList(),
            RecentAuditEvents = _mapper.Map<List<AuditLogDto>>(recentAudits)
        };

        return summary;
    }
}

