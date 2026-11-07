using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IReportRepository : IRepository<ReportTemplate>
{
    // [SECURITY-TEST-SCENARIO] Exercises System.Linq.Dynamic.Core 1.2.18 (CVE-2023-32571 / GHSA-w65q-jcmv-28gj)
    Task<IReadOnlyList<Employee>> QueryEmployeesDynamicLinqAsync(string dynamicWhere, string dynamicOrderBy, CancellationToken cancellationToken = default);

    // [SECURITY-TEST-SCENARIO] CWE-89: Raw SQL query (requires SQL Server provider)
    Task<IReadOnlyList<Employee>> ExecuteCustomReportRawSqlAsync(string rawWhereClause, CancellationToken cancellationToken = default);
}

