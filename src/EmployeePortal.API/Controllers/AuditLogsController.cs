using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IAdminAuditNotificationService _adminAuditService;

    public AuditLogsController(IAdminAuditNotificationService adminAuditService)
    {
        _adminAuditService = adminAuditService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetLogs(
        [FromQuery] int count = 50,
        [FromQuery] string? moduleFilter = null,
        CancellationToken cancellationToken = default)
    {
        var logs = await _adminAuditService.GetAuditLogsAsync(count, moduleFilter, cancellationToken);
        return Ok(logs);
    }
}

