using EmployeePortal.Data.Context;
using EmployeePortal.Data.Repositories;
using EmployeePortal.Integrations.Payroll;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeePortal.IntegrationTests;

public sealed class SqlServerFactAttribute : FactAttribute
{
    public const string EnvVarName = "PORTAL_SQLSERVER_TEST_CS";

    public SqlServerFactAttribute()
    {
        var cs = Environment.GetEnvironmentVariable(EnvVarName);
        if (string.IsNullOrWhiteSpace(cs))
        {
            Skip = $"Skipped: Set {EnvVarName} to a live SQL Server connection string to execute relational SQL Server security tests (FromSqlRaw / SqlCommand).";
        }
    }
}

public class SqlServerRelationalSecurityTests
{
    [SqlServerFact]
    public async Task SearchEmployeesRawSqlAsync_AgainstSqlServer_ExecutesRelationalRawQuery()
    {
        var connectionString = Environment.GetEnvironmentVariable(SqlServerFactAttribute.EnvVarName)!;
        var options = new DbContextOptionsBuilder<PortalDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var dbContext = new PortalDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        await PortalDbSeeder.SeedAsync(dbContext);

        var repository = new EmployeeRepository(dbContext);
        var results = await repository.SearchEmployeesRawSqlAsync("Vance");
        Assert.NotEmpty(results);
    }

    [SqlServerFact]
    public async Task ExportPayrollViaLegacyAdoSqlAsync_AgainstSqlServer_ExecutesSqlCommand()
    {
        var connectionString = Environment.GetEnvironmentVariable(SqlServerFactAttribute.EnvVarName)!;
        var bridge = new XmlPayrollExportBridge();
        var rows = await bridge.ExportPayrollViaLegacyAdoSqlAsync(connectionString, "Processed");
        Assert.NotNull(rows);
    }
}

