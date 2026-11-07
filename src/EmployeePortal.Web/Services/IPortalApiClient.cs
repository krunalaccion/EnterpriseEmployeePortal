using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Web.Services;

public interface IPortalApiClient
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<PagedResultDto<EmployeeDto>> SearchEmployeesAsync(SearchQueryDto query, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> CreateEmployeeAsync(UpsertEmployeeDto dto, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpsertEmployeeDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<DepartmentDto?> CreateDepartmentAsync(UpsertDepartmentDto dto, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccountDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignRoleAsync(AssignUserRoleDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PayrollRecordDto>> GetPayrollHistoryAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<PayrollRecordDto?> GetPayslipByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PayrollRecordDto?> ProcessPayrollRunAsync(CreatePayrollRunDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto?> RecordShiftAsync(ClockInOutRequestDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequestDto>> GetLeaveRequestsAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto?> SubmitLeaveRequestAsync(SubmitLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto?> ReviewLeaveRequestAsync(ReviewLeaveRequestDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmployeeDocumentDto>> GetDocumentsAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<EmployeeDocumentDto?> UploadDocumentAsync(UploadDocumentRequestDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportTemplateDto>> GetReportTemplatesAsync(CancellationToken cancellationToken = default);
    Task<WorkforceAnalyticsReportDto?> RunDynamicReportAsync(DynamicReportQueryDto query, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(int count = 50, string? moduleFilter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(CancellationToken cancellationToken = default);
    Task<NotificationDto?> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemSettingDto>> GetSystemSettingsAsync(CancellationToken cancellationToken = default);
    Task<SystemSettingDto?> UpsertSystemSettingAsync(UpdateSystemSettingDto dto, CancellationToken cancellationToken = default);
}

