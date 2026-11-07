using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IAdminAuditNotificationService _adminAuditService;

    public NotificationsController(IAdminAuditNotificationService adminAuditService)
    {
        _adminAuditService = adminAuditService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetNotifications(
        [FromQuery] int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var list = await _adminAuditService.GetNotificationsAsync(userId, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create(
        [FromBody] CreateNotificationDto dto,
        CancellationToken cancellationToken)
    {
        var created = await _adminAuditService.CreateNotificationAsync(dto, cancellationToken);
        return Ok(created);
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        await _adminAuditService.MarkNotificationReadAsync(id, cancellationToken);
        return NoContent();
    }
}

