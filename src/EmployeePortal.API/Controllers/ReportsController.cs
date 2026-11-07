using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IDocumentAndReportService _documentAndReportService;

    public ReportsController(IDocumentAndReportService documentAndReportService)
    {
        _documentAndReportService = documentAndReportService;
    }

    [HttpGet("templates")]
    public async Task<ActionResult<IReadOnlyList<ReportTemplateDto>>> GetTemplates(CancellationToken cancellationToken)
    {
        var templates = await _documentAndReportService.GetReportTemplatesAsync(cancellationToken);
        return Ok(templates);
    }

    [HttpPost("dynamic-query")]
    public async Task<ActionResult<WorkforceAnalyticsReportDto>> RunDynamicLinqReport(
        [FromBody] DynamicReportQueryDto query,
        CancellationToken cancellationToken)
    {
        var report = await _documentAndReportService.RunDynamicLinqReportAsync(query, cancellationToken);
        return Ok(report);
    }

    // [SECURITY-TEST-SCENARIO] ID: VULN-SQLI-03 | CWE-89: Raw SQL Report Execution (SQL Server only)
    [HttpPost("execute-raw")]
    public async Task<ActionResult<WorkforceAnalyticsReportDto>> RunRawSqlReport(
        [FromBody] DynamicReportQueryDto query,
        CancellationToken cancellationToken)
    {
        var report = await _documentAndReportService.RunRawSqlReportAsync(query.RawSqlFilter, cancellationToken);
        return Ok(report);
    }
}

