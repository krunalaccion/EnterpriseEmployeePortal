using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Business.Validation;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;

namespace EmployeePortal.Business.Services;

public class AttendanceAndLeaveService : IAttendanceAndLeaveService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ILeaveRepository _leaveRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IInputValidationService _validationService;

    public AttendanceAndLeaveService(
        IAttendanceRepository attendanceRepository,
        ILeaveRepository leaveRepository,
        IEmployeeRepository employeeRepository,
        IInputValidationService validationService)
    {
        _attendanceRepository = attendanceRepository;
        _leaveRepository = leaveRepository;
        _employeeRepository = employeeRepository;
        _validationService = validationService;
    }

    public async Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var records = await _attendanceRepository.GetAttendanceRecordsWithEmployeeAsync(employeeId, cancellationToken);
        return records.Select(a => new AttendanceRecordDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee != null ? $"{a.Employee.FirstName} {a.Employee.LastName}" : $"Employee #{a.EmployeeId}",
            WorkDate = a.WorkDate,
            ClockInUtc = a.ClockInUtc,
            ClockOutUtc = a.ClockOutUtc,
            RegularHours = a.RegularHours,
            OvertimeHours = a.OvertimeHours,
            AttendanceStatus = a.AttendanceStatus,
            SourceIpAddress = a.SourceIpAddress,
            ShiftNotes = a.ShiftNotes
        }).ToList();
    }

    public async Task<AttendanceRecordDto> RecordShiftAsync(ClockInOutRequestDto dto, string clientIp, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId, cancellationToken)
            ?? throw new InvalidOperationException($"Employee ID {dto.EmployeeId} not found.");

        var now = DateTime.UtcNow;
        var entity = new AttendanceRecord
        {
            EmployeeId = dto.EmployeeId,
            WorkDate = now.Date,
            ClockInUtc = now.AddHours(-8),
            ClockOutUtc = now,
            RegularHours = 8.0m,
            OvertimeHours = dto.OvertimeHours,
            AttendanceStatus = "Present",
            SourceIpAddress = clientIp,
            ShiftNotes = dto.ShiftNotes
        };

        var saved = await _attendanceRepository.AddAsync(entity, cancellationToken);
        return new AttendanceRecordDto
        {
            Id = saved.Id,
            EmployeeId = saved.EmployeeId,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            WorkDate = saved.WorkDate,
            ClockInUtc = saved.ClockInUtc,
            ClockOutUtc = saved.ClockOutUtc,
            RegularHours = saved.RegularHours,
            OvertimeHours = saved.OvertimeHours,
            AttendanceStatus = saved.AttendanceStatus,
            SourceIpAddress = saved.SourceIpAddress,
            ShiftNotes = saved.ShiftNotes
        };
    }

    public async Task<IReadOnlyList<LeaveRequestDto>> GetLeaveRequestsAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var requests = await _leaveRepository.GetLeaveRequestsWithEmployeeAsync(employeeId, cancellationToken);
        return requests.Select(MapLeaveToDto).ToList();
    }

    public async Task<LeaveRequestDto> SubmitLeaveRequestAsync(SubmitLeaveRequestDto dto, CancellationToken cancellationToken = default)
    {
        var (isValid, error) = _validationService.ValidateLeaveDates(dto.StartDate, dto.EndDate);
        if (!isValid)
        {
            throw new ArgumentException(error);
        }

        var employee = await _employeeRepository.GetEmployeeWithDetailsAsync(dto.EmployeeId, cancellationToken)
            ?? throw new InvalidOperationException($"Employee ID {dto.EmployeeId} not found.");

        var totalDays = (decimal)((dto.EndDate.Date - dto.StartDate.Date).TotalDays + 1);
        var entity = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate.Date,
            EndDate = dto.EndDate.Date,
            TotalDays = totalDays,
            Reason = dto.Reason,
            Status = LeaveApprovalStatus.Pending
        };

        var created = await _leaveRepository.AddAsync(entity, cancellationToken);
        created.Employee = employee;
        return MapLeaveToDto(created);
    }

    public async Task<LeaveRequestDto?> ReviewLeaveRequestAsync(ReviewLeaveRequestDto dto, int reviewerUserId, CancellationToken cancellationToken = default)
    {
        var existing = await _leaveRepository.GetByIdAsync(dto.LeaveRequestId, cancellationToken);
        if (existing == null)
        {
            return null;
        }

        existing.Status = dto.NewStatus;
        existing.ReviewerComments = dto.ReviewerComments;
        existing.ApprovedByUserId = reviewerUserId;
        existing.ReviewedAtUtc = DateTime.UtcNow;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _leaveRepository.UpdateAsync(existing, cancellationToken);
        var all = await _leaveRepository.GetLeaveRequestsWithEmployeeAsync(existing.EmployeeId, cancellationToken);
        var reloaded = all.FirstOrDefault(r => r.Id == existing.Id) ?? existing;
        return MapLeaveToDto(reloaded);
    }

    private static LeaveRequestDto MapLeaveToDto(LeaveRequest l)
    {
        return new LeaveRequestDto
        {
            Id = l.Id,
            EmployeeId = l.EmployeeId,
            EmployeeName = l.Employee != null ? $"{l.Employee.FirstName} {l.Employee.LastName}" : $"Employee #{l.EmployeeId}",
            DepartmentName = l.Employee?.Department?.Name ?? "General",
            LeaveType = l.LeaveType,
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            TotalDays = l.TotalDays,
            Reason = l.Reason,
            Status = l.Status,
            ReviewerComments = l.ReviewerComments,
            ReviewedAtUtc = l.ReviewedAtUtc
        };
    }
}

