using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(PortalDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbContext.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Manager)
                .OrderBy(e => e.LastName)
                .ToListAsync(cancellationToken);
        }


// ... [Omitted 31 lines of unrelated methods to compress context] ...


        if (!string.IsNullOrWhiteSpace(query.StatusFilter) &&
            Enum.TryParse<EmploymentStatus>(query.StatusFilter, true, out var parsedStatus))
        {
            baseQuery = baseQuery.Where(e => e.Status == parsedStatus);
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var page = Math.Max(query.PageNumber, 1);
        var size = Math.Clamp(query.PageSize, 1, 100);

        var items = await baseQuery
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<Employee>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size
        };
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-SQLI-01 | CWE-89: Improper Neutralization of Special Elements used in an SQL Command ('SQL Injection')
    // WARNING: INTENTIONALLY VULNERABLE RAW SQL QUERY FOR SECURESCANAI BENCHMARKING ONLY.
    // Concatenates untrusted rawFilter directly into FromSqlRaw.
    // Note: Requires the relational SQL Server provider; EF Core InMemory provider does not execute relational SQL.
    // =========================================================================
    public async Task<IReadOnlyList<Employee>> SearchEmployeesRawSqlAsync(string rawFilter, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Employees WHERE LastName LIKE @p0 OR JobTitle LIKE @p0";
        var parameters = new object[] { $"%{rawFilter}%", $"%{rawFilter}%" };
        return await DbContext.Employees
            .FromSqlRaw(sql, parameters)
            .Include(e => e.Department)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
