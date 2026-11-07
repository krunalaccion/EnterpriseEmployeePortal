using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveController : ControllerBase
{
    private readonly IAttendanceAndLeaveService _attendanceAndLeaveService;

    public LeaveController(IAttendanceAndLeaveService attendanceAndLeaveService)
    {
        _attendanceAndLeaveService = attendanceAndLeaveService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestDto>>> GetLeaveRequests(
        [FromQuery] int? employeeId,
        CancellationToken cancellationToken)
    {
        var list = await _attendanceAndLeaveService.GetLeaveRequestsAsync(employeeId, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> SubmitLeaveRequest(
        [FromBody] SubmitLeaveRequestDto dto,
        CancellationToken cancellationToken)
    {
        var created = await _attendanceAndLeaveService.SubmitLeaveRequestAsync(dto, cancellationToken);
        return Ok(created);
    }

    [HttpPut("review")]
    public async Task<ActionResult<LeaveRequestDto>> ReviewLeaveRequest(
        [FromBody] ReviewLeaveRequestDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _attendanceAndLeaveService.ReviewLeaveRequestAsync(dto, reviewerUserId: 1, cancellationToken);
        return updated == null ? NotFound() : Ok(updated);
    }
}

