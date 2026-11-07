using System.Text;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PayrollRecordDto>>> GetHistory(
        [FromQuery] int? employeeId,
        CancellationToken cancellationToken)
    {
        var records = await _payrollService.GetPayrollHistoryAsync(employeeId, cancellationToken);
        return Ok(records);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-IDOR-01 | CWE-639: Authorization Bypass Through User-Controlled Key (IDOR)
    // WARNING: INTENTIONALLY VULNERABLE ENDPOINT FOR SECURESCANAI BENCHMARKING ONLY.
    // Returns any employee's confidential payslip by primary key without verifying caller ownership.
    // =========================================================================
    [HttpGet("payslips/{id:int}")]
    public async Task<ActionResult<PayrollRecordDto>> GetPayslipById(int id, CancellationToken cancellationToken)
    {
        var payslip = await _payrollService.GetPayslipByIdAsync(id, cancellationToken);
        return payslip == null ? NotFound() : Ok(payslip);
    }

    [HttpPost("process")]
    public async Task<ActionResult<PayrollRecordDto>> ProcessPayrollRun(
        [FromBody] CreatePayrollRunDto dto,
        CancellationToken cancellationToken)
    {
        var actor = User.Identity?.Name ?? "payroll-system";
        var record = await _payrollService.ProcessPayrollRunAsync(dto, actor, cancellationToken);
        return Ok(record);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-AUTHZ-01 | CWE-862: Missing Authorization
    // WARNING: INTENTIONALLY VULNERABLE ENDPOINT FOR SECURESCANAI BENCHMARKING ONLY.
    // Exports the full company ACH bank payroll file without requiring [Authorize(Roles = "Admin,PayrollSpecialist")].
    // =========================================================================
    [HttpPost("export-bank-file")]
    public async Task<IActionResult> ExportBankAchFile(CancellationToken cancellationToken)
    {
        var xml = await _payrollService.ExportBankAchXmlAsync(cancellationToken);
        return File(Encoding.UTF8.GetBytes(xml), "application/xml", "payroll-ach-batch.xml");
    }

    // [SECURITY-TEST-SCENARIO] ID: VULN-DESER-02 | CWE-611 / CWE-502: XML External Entity / DTD Parsing
    [HttpPost("import-bank-ack")]
    public ActionResult<IReadOnlyList<string>> ImportBankAckXml([FromBody] PayrollXmlImportDto dto)
    {
        var parsedNodes = _payrollService.ImportBankAckXmlUnsafe(dto.XmlContent);
        return Ok(parsedNodes);
    }
}

