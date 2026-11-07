using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var list = await _departmentService.GetAllDepartmentsAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var dept = await _departmentService.GetDepartmentByIdAsync(id, cancellationToken);
        return dept == null ? NotFound() : Ok(dept);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] UpsertDepartmentDto dto, CancellationToken cancellationToken)
    {
        var created = await _departmentService.CreateDepartmentAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> Update(int id, [FromBody] UpsertDepartmentDto dto, CancellationToken cancellationToken)
    {
        var updated = await _departmentService.UpdateDepartmentAsync(id, dto, cancellationToken);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _departmentService.DeleteDepartmentAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

