using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceAndLeaveService _attendanceAndLeaveService;

    public AttendanceController(IAttendanceAndLeaveService attendanceAndLeaveService)
    {
        _attendanceAndLeaveService = attendanceAndLeaveService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecordDto>>> GetAttendance(
        [FromQuery] int? employeeId,
        CancellationToken cancellationToken)
    {
        var list = await _attendanceAndLeaveService.GetAttendanceRecordsAsync(employeeId, cancellationToken);
        return Ok(list);
    }

    [HttpPost("clock")]
    public async Task<ActionResult<AttendanceRecordDto>> RecordShift(
        [FromBody] ClockInOutRequestDto dto,
        CancellationToken cancellationToken)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var record = await _attendanceAndLeaveService.RecordShiftAsync(dto, clientIp, cancellationToken);
        return Ok(record);
    }
}

