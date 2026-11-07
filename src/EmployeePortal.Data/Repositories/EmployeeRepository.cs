using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

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

    public async Task<Employee?> GetEmployeeWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<PagedResultDto<Employee>> SearchEmployeesAsync(SearchQueryDto query, CancellationToken cancellationToken = default)
    {
        var baseQuery = DbContext.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Manager)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLowerInvariant();
            baseQuery = baseQuery.Where(e =>
                e.FirstName.ToLower().Contains(term) ||
                e.LastName.ToLower().Contains(term) ||
                e.Email.ToLower().Contains(term) ||
                e.EmployeeNumber.ToLower().Contains(term) ||
                e.JobTitle.ToLower().Contains(term));
        }

        if (query.DepartmentId.HasValue && query.DepartmentId.Value > 0)
        {
            baseQuery = baseQuery.Where(e => e.DepartmentId == query.DepartmentId.Value);
        }

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
        var unsafeSql = $"SELECT * FROM Employees WHERE LastName LIKE '%{rawFilter}%' OR JobTitle LIKE '%{rawFilter}%'";
        return await DbContext.Employees
            .FromSqlRaw(unsafeSql)
            .Include(e => e.Department)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

