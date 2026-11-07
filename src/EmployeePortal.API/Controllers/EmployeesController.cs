using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<EmployeeDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int? departmentId,
        [FromQuery] string? statusFilter,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchQueryDto
        {
            SearchTerm = searchTerm,
            DepartmentId = departmentId,
            StatusFilter = statusFilter,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _employeeService.SearchEmployeesAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id, cancellationToken);
        return employee == null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] UpsertEmployeeDto dto, CancellationToken cancellationToken)
    {
        var actor = User.Identity?.Name ?? "anonymous";
        var created = await _employeeService.CreateEmployeeAsync(dto, actor, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpsertEmployeeDto dto, CancellationToken cancellationToken)
    {
        var actor = User.Identity?.Name ?? "anonymous";
        var updated = await _employeeService.UpdateEmployeeAsync(id, dto, actor, cancellationToken);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var actor = User.Identity?.Name ?? "anonymous";
        var deleted = await _employeeService.DeleteEmployeeAsync(id, actor, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    // [SECURITY-TEST-SCENARIO] ID: VULN-SQLI-01 | CWE-89: Raw SQL Injection endpoint (SQL Server only)
    [HttpGet("raw-search")]
    public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> SearchRawSql([FromQuery] string filter, CancellationToken cancellationToken)
    {
        var results = await _employeeService.SearchEmployeesRawSqlAsync(filter ?? string.Empty, cancellationToken);
        return Ok(results);
    }
}

