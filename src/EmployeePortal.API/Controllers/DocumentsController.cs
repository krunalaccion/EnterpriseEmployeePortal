using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentAndReportService _documentAndReportService;

    public DocumentsController(IDocumentAndReportService documentAndReportService)
    {
        _documentAndReportService = documentAndReportService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeDocumentDto>>> GetAll(
        [FromQuery] int? employeeId,
        CancellationToken cancellationToken)
    {
        var docs = await _documentAndReportService.GetDocumentsAsync(employeeId, cancellationToken);
        return Ok(docs);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-IDOR-02 | CWE-639: IDOR on Confidential Employee Documents
    // Returns document metadata by ID without verifying caller ownership or confidential access rights.
    // =========================================================================
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDocumentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var doc = await _documentAndReportService.GetDocumentMetadataByIdAsync(id, cancellationToken);
        return doc == null ? NotFound() : Ok(doc);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-UPLOAD-01 | CWE-434: Unrestricted Upload of File with Dangerous Type
    // WARNING: INTENTIONALLY VULNERABLE FILE UPLOAD FOR SECURESCANAI BENCHMARKING ONLY.
    // Accepts any file extension (.aspx, .exe, .ps1, .html, .dll) without allow-listing or content inspection.
    // =========================================================================
    [HttpPost("upload")]
    public async Task<ActionResult<EmployeeDocumentDto>> Upload(
        [FromBody] UploadDocumentRequestDto dto,
        CancellationToken cancellationToken)
    {
        var uploader = User.Identity?.Name ?? "anonymous";
        var saved = await _documentAndReportService.UploadDocumentAsync(dto, uploader, cancellationToken);
        return Ok(saved);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-PATH-01 | CWE-22: Path Traversal File Download
    // WARNING: INTENTIONALLY VULNERABLE ENDPOINT FOR SECURESCANAI BENCHMARKING ONLY.
    // Passes raw fileName query parameter directly to unsanitized file system read.
    // =========================================================================
    [HttpGet("download-raw")]
    public async Task<IActionResult> DownloadRaw([FromQuery] string fileName, CancellationToken cancellationToken)
    {
        var bytes = await _documentAndReportService.DownloadDocumentUnsanitizedAsync(fileName ?? string.Empty, cancellationToken);
        if (bytes.Length == 0)
        {
            return NotFound(new { Message = $"File '{fileName}' not found." });
        }

        return File(bytes, "application/octet-stream", Path.GetFileName(fileName));
    }
}

