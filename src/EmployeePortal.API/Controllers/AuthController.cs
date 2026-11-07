using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthAndUserService _authAndUserService;

    public AuthController(IAuthAndUserService authAndUserService)
    {
        _authAndUserService = authAndUserService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authAndUserService.LoginAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
}

