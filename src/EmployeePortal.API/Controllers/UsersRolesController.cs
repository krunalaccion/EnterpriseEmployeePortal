using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersRolesController : ControllerBase
{
    private readonly IAuthAndUserService _authAndUserService;

    public UsersRolesController(IAuthAndUserService authAndUserService)
    {
        _authAndUserService = authAndUserService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<UserAccountDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _authAndUserService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await _authAndUserService.GetAllRolesAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignUserRoleDto dto, CancellationToken cancellationToken)
    {
        var actor = User.Identity?.Name ?? "anonymous";
        await _authAndUserService.AssignRoleAsync(dto, actor, cancellationToken);
        return Ok(new { Message = $"Role '{dto.RoleName}' assigned to user #{dto.UserId}." });
    }
}

