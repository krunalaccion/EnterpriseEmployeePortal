using System.Linq.Dynamic.Core;
using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class ReportRepository : GenericRepository<ReportTemplate>, IReportRepository
{
    public ReportRepository(PortalDbContext dbContext) : base(dbContext)
    {
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-DEP-DYNLINQ | CVE-2023-32571 / GHSA-w65q-jcmv-28gj (CWE-94)
    // WARNING: INTENTIONALLY VULNERABLE DYNAMIC LINQ QUERY FOR SECURESCANAI BENCHMARKING ONLY.
    // Passes unsanitized user filter/orderBy expressions into System.Linq.Dynamic.Core 1.2.18.
    // =========================================================================
    public async Task<IReadOnlyList<Employee>> QueryEmployeesDynamicLinqAsync(
        string dynamicWhere,
        string dynamicOrderBy,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> query = DbContext.Employees
            .AsNoTracking()
            .Include(e => e.Department);

        if (!string.IsNullOrWhiteSpace(dynamicWhere))
        {
            query = query.Where(dynamicWhere);
        }

        var sortExpr = string.IsNullOrWhiteSpace(dynamicOrderBy) ? "LastName ascending" : dynamicOrderBy;
        query = query.OrderBy(sortExpr);

        return await query.ToListAsync(cancellationToken);
    }

    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-SQLI-03 | CWE-89: Raw SQL Injection in Custom Report Builder
    // WARNING: INTENTIONALLY VULNERABLE RAW SQL QUERY FOR SECURESCANAI BENCHMARKING ONLY.
    // Note: Requires SQL Server relational provider; throws on EF Core InMemory provider.
    // =========================================================================
    public async Task<IReadOnlyList<Employee>> ExecuteCustomReportRawSqlAsync(string rawWhereClause, CancellationToken cancellationToken = default)
    {
        var whereSegment = string.IsNullOrWhiteSpace(rawWhereClause) ? "1 = 1" : rawWhereClause;
        var sql = $"SELECT * FROM Employees WHERE {whereSegment}";
        return await DbContext.Employees
            .FromSqlRaw(sql)
            .Include(e => e.Department)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

