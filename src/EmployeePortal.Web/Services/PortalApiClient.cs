using System.Net.Http.Headers;
using System.Net.Http.Json;
using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Web.Services;

/// <summary>
/// Communicates with EmployeePortal.API strictly over HTTP REST endpoints.
/// Does not reference EmployeePortal.Data or EmployeePortal.Business.
/// </summary>
public class PortalApiClient : IPortalApiClient
{
    private readonly HttpClient _httpClient;
    private readonly PortalUserSessionService _sessionService;

    public PortalApiClient(HttpClient httpClient, PortalUserSessionService sessionService)
    {
        _httpClient = httpClient;
        _sessionService = sessionService;
    }

    private void AttachAuthHeader()
    {
        if (!string.IsNullOrWhiteSpace(_sessionService.JwtToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _sessionService.JwtToken);
        }
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<DashboardSummaryDto>("api/dashboard", cancellationToken)
               ?? new DashboardSummaryDto();
    }

    public async Task<PagedResultDto<EmployeeDto>> SearchEmployeesAsync(SearchQueryDto query, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var url = $"api/employees?searchTerm={Uri.EscapeDataString(query.SearchTerm ?? string.Empty)}&departmentId={query.DepartmentId}&statusFilter={Uri.EscapeDataString(query.StatusFilter ?? string.Empty)}&pageNumber={query.PageNumber}&pageSize={query.PageSize}";
        return await _httpClient.GetFromJsonAsync<PagedResultDto<EmployeeDto>>(url, cancellationToken)
               ?? new PagedResultDto<EmployeeDto>();
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<EmployeeDto>($"api/employees/{id}", cancellationToken);
    }

    public async Task<EmployeeDto?> CreateEmployeeAsync(UpsertEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/employees", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDto>(cancellationToken: cancellationToken);
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpsertEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/employees/{id}", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDto>(cancellationToken: cancellationToken);
    }

    public async Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/employees/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<DepartmentDto>>("api/departments", cancellationToken)
               ?? new List<DepartmentDto>();
    }

    public async Task<DepartmentDto?> CreateDepartmentAsync(UpsertDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/departments", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DepartmentDto>(cancellationToken: cancellationToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        return await response.Content.ReadFromJsonAsync<AuthResponseDto>(cancellationToken: cancellationToken)
               ?? new AuthResponseDto { Succeeded = false, Message = "Unable to parse login response." };
    }

    public async Task<IReadOnlyList<UserAccountDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<UserAccountDto>>("api/usersroles/users", cancellationToken)
               ?? new List<UserAccountDto>();
    }

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/usersroles/roles", cancellationToken)
               ?? new List<RoleDto>();
    }

    public async Task<bool> AssignRoleAsync(AssignUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/usersroles/assign-role", dto, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<IReadOnlyList<PayrollRecordDto>> GetPayrollHistoryAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var url = employeeId.HasValue ? $"api/payroll?employeeId={employeeId.Value}" : "api/payroll";
        return await _httpClient.GetFromJsonAsync<List<PayrollRecordDto>>(url, cancellationToken)
               ?? new List<PayrollRecordDto>();
    }

    public async Task<PayrollRecordDto?> GetPayslipByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<PayrollRecordDto>($"api/payroll/payslips/{id}", cancellationToken);
    }

    public async Task<PayrollRecordDto?> ProcessPayrollRunAsync(CreatePayrollRunDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/payroll/process", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PayrollRecordDto>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var url = employeeId.HasValue ? $"api/attendance?employeeId={employeeId.Value}" : "api/attendance";
        return await _httpClient.GetFromJsonAsync<List<AttendanceRecordDto>>(url, cancellationToken)
               ?? new List<AttendanceRecordDto>();
    }

    public async Task<AttendanceRecordDto?> RecordShiftAsync(ClockInOutRequestDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/attendance/clock", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AttendanceRecordDto>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveRequestDto>> GetLeaveRequestsAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var url = employeeId.HasValue ? $"api/leave?employeeId={employeeId.Value}" : "api/leave";
        return await _httpClient.GetFromJsonAsync<List<LeaveRequestDto>>(url, cancellationToken)
               ?? new List<LeaveRequestDto>();
    }

    public async Task<LeaveRequestDto?> SubmitLeaveRequestAsync(SubmitLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/leave", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LeaveRequestDto>(cancellationToken: cancellationToken);
    }

    public async Task<LeaveRequestDto?> ReviewLeaveRequestAsync(ReviewLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PutAsJsonAsync("api/leave/review", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LeaveRequestDto>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<EmployeeDocumentDto>> GetDocumentsAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var url = employeeId.HasValue ? $"api/documents?employeeId={employeeId.Value}" : "api/documents";
        return await _httpClient.GetFromJsonAsync<List<EmployeeDocumentDto>>(url, cancellationToken)
               ?? new List<EmployeeDocumentDto>();
    }

    public async Task<EmployeeDocumentDto?> UploadDocumentAsync(UploadDocumentRequestDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/documents/upload", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDocumentDto>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<ReportTemplateDto>> GetReportTemplatesAsync(CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<ReportTemplateDto>>("api/reports/templates", cancellationToken)
               ?? new List<ReportTemplateDto>();
    }

    public async Task<WorkforceAnalyticsReportDto?> RunDynamicReportAsync(DynamicReportQueryDto query, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/reports/dynamic-query", query, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkforceAnalyticsReportDto>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(int count = 50, string? moduleFilter = null, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var url = $"api/auditlogs?count={count}&moduleFilter={Uri.EscapeDataString(moduleFilter ?? string.Empty)}";
        return await _httpClient.GetFromJsonAsync<List<AuditLogDto>>(url, cancellationToken)
               ?? new List<AuditLogDto>();
    }

    public async Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<NotificationDto>>("api/notifications", cancellationToken)
               ?? new List<NotificationDto>();
    }

    public async Task<NotificationDto?> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/notifications", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NotificationDto>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<SystemSettingDto>> GetSystemSettingsAsync(CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<SystemSettingDto>>("api/administration/settings", cancellationToken)
               ?? new List<SystemSettingDto>();
    }

    public async Task<SystemSettingDto?> UpsertSystemSettingAsync(UpdateSystemSettingDto dto, CancellationToken cancellationToken = default)
    {
        AttachAuthHeader();
        var response = await _httpClient.PutAsJsonAsync("api/administration/settings", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SystemSettingDto>(cancellationToken: cancellationToken);
    }
}

