using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IEmployeeService
{
    Task<PagedResultDto<EmployeeDto>> SearchEmployeesAsync(SearchQueryDto query, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> CreateEmployeeAsync(UpsertEmployeeDto dto, string actorUsername, CancellationToken cancellationToken = default);
    Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpsertEmployeeDto dto, string actorUsername, CancellationToken cancellationToken = default);
    Task<bool> DeleteEmployeeAsync(int id, string actorUsername, CancellationToken cancellationToken = default);

    // [SECURITY-TEST-SCENARIO] CWE-89: Raw SQL search (SQL Server only)
    Task<IReadOnlyList<EmployeeDto>> SearchEmployeesRawSqlAsync(string rawFilter, CancellationToken cancellationToken = default);
}

