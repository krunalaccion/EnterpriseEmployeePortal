using EmployeePortal.Business.Security;
using EmployeePortal.Business.Services;
using EmployeePortal.Business.Validation;
using EmployeePortal.Data.Repositories;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Enums;
using EmployeePortal.UnitTests.Fixtures;
using Xunit;

namespace EmployeePortal.UnitTests;

public class EmployeeServiceTests
{
    [Fact]
    public async Task SearchEmployeesAsync_WithSearchTerm_FiltersCorrectly()
    {
        await using var db = await InMemoryPortalTestFixture.CreateSeededDbContextAsync();
        var service = new EmployeeService(
            new EmployeeRepository(db),
            new UserRoleRepository(db),
            new AuditLogRepository(db),
            new LegacyCryptoHelper(),
            new InputValidationService(),
            InMemoryPortalTestFixture.CreateMapper());

        var result = await service.SearchEmployeesAsync(new SearchQueryDto { SearchTerm = "Vance", PageNumber = 1, PageSize = 10 });

        Assert.Single(result.Items);
        Assert.Equal("EMP-1001", result.Items[0].EmployeeNumber);
        Assert.NotEmpty(result.Items[0].EncryptedTaxId);
    }

    [Fact]
    public async Task CreateAndUpdateEmployeeAsync_PersistsChangesAndAuditLog()
    {
        await using var db = await InMemoryPortalTestFixture.CreateSeededDbContextAsync();
        var service = new EmployeeService(
            new EmployeeRepository(db),
            new UserRoleRepository(db),
            new AuditLogRepository(db),
            new LegacyCryptoHelper(),
            new InputValidationService(),
            InMemoryPortalTestFixture.CreateMapper());

        var created = await service.CreateEmployeeAsync(new UpsertEmployeeDto
        {
            EmployeeNumber = "EMP-2050",
            FirstName = "Jordan",
            LastName = "Mercer",
            Email = "jordan.mercer@dummy-hr-portal.local",
            JobTitle = "Staff SRE",
            DepartmentId = 1,
            BaseSalary = 165000m,
            PlaintextTaxId = "999-50-2050",
            Status = EmploymentStatus.Active
        }, "unit-test-admin");

        Assert.True(created.Id > 0);
        Assert.Equal("Jordan Mercer", created.FullName);

        var updated = await service.UpdateEmployeeAsync(created.Id, new UpsertEmployeeDto
        {
            EmployeeNumber = "EMP-2050",
            FirstName = "Jordan",
            LastName = "Mercer",
            Email = "jordan.mercer@dummy-hr-portal.local",
            JobTitle = "Principal SRE",
            DepartmentId = 1,
            BaseSalary = 180000m,
            Status = EmploymentStatus.Active
        }, "unit-test-admin");

        Assert.NotNull(updated);
        Assert.Equal("Principal SRE", updated!.JobTitle);
        Assert.Equal(180000m, updated.BaseSalary);
    }
}

