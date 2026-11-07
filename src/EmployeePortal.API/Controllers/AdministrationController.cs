using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdministrationController : ControllerBase
{
    private readonly IAdminAuditNotificationService _adminAuditService;

    public AdministrationController(IAdminAuditNotificationService adminAuditService)
    {
        _adminAuditService = adminAuditService;
    }

    [HttpGet("settings")]
    public async Task<ActionResult<IReadOnlyList<SystemSettingDto>>> GetSettings(CancellationToken cancellationToken)
    {
        var settings = await _adminAuditService.GetSystemSettingsAsync(cancellationToken);
        return Ok(settings);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-AUTHZ-02 | CWE-862: Missing Authorization on System Configuration
    // WARNING: INTENTIONALLY VULNERABLE ENDPOINT FOR SECURESCANAI BENCHMARKING ONLY.
    // Allows unauthenticated callers to modify system-wide security settings.
    // =========================================================================
    [HttpPut("settings")]
    public async Task<ActionResult<SystemSettingDto>> UpsertSetting(
        [FromBody] UpdateSystemSettingDto dto,
        CancellationToken cancellationToken)
    {
        var actor = User.Identity?.Name ?? "unauthenticated-caller";
        var updated = await _adminAuditService.UpsertSystemSettingAsync(dto, actor, cancellationToken);
        return Ok(updated);
    }

    [HttpGet("export-state")]
    public async Task<ActionResult<StateSnapshotPayloadDto>> ExportState(CancellationToken cancellationToken)
    {
        var json = await _adminAuditService.ExportSystemStateSnapshotAsync(cancellationToken);
        return Ok(new StateSnapshotPayloadDto { SerializedJsonState = json });
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-DESER-01 | CWE-502: Insecure Deserialization of Untrusted State Snapshot
    // WARNING: INTENTIONALLY VULNERABLE ENDPOINT FOR SECURESCANAI BENCHMARKING ONLY.
    // Deserializes untrusted JSON with TypeNameHandling.All via LegacyJsonSerializer.
    // =========================================================================
    [HttpPost("import-state")]
    public IActionResult ImportState([FromBody] StateSnapshotPayloadDto dto)
    {
        var deserialized = _adminAuditService.ImportSystemStateSnapshotUnsafe(dto.SerializedJsonState);
        return Ok(new
        {
            Message = "State snapshot processed.",
            ResolvedType = deserialized?.GetType().FullName ?? "null"
        });
    }
}

