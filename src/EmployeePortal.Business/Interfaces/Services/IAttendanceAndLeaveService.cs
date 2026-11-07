using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IAttendanceAndLeaveService
{
    Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto> RecordShiftAsync(ClockInOutRequestDto dto, string clientIp, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequestDto>> GetLeaveRequestsAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto> SubmitLeaveRequestAsync(SubmitLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto?> ReviewLeaveRequestAsync(ReviewLeaveRequestDto dto, int reviewerUserId, CancellationToken cancellationToken = default);
}

