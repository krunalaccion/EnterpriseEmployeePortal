using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<PagedResultDto<Employee>> SearchEmployeesAsync(SearchQueryDto query, CancellationToken cancellationToken = default);
    Task<Employee?> GetEmployeeWithDetailsAsync(int id, CancellationToken cancellationToken = default);

    // [SECURITY-TEST-SCENARIO] CWE-89: SQL Injection test contract (requires relational SQL Server provider)
    Task<IReadOnlyList<Employee>> SearchEmployeesRawSqlAsync(string rawFilter, CancellationToken cancellationToken = default);
}

