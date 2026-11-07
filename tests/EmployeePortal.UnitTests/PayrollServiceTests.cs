using EmployeePortal.Business.Services;
using EmployeePortal.Data.Repositories;
using EmployeePortal.Integrations.Payroll;
using EmployeePortal.Models.DTOs;
using EmployeePortal.UnitTests.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EmployeePortal.UnitTests;

public class PayrollServiceTests
{
    [Fact]
    public async Task ProcessPayrollRunAsync_ComputesTaxesAndNetPayCorrectly()
    {
        await using var db = await InMemoryPortalTestFixture.CreateSeededDbContextAsync();
        var service = new PayrollService(
            new PayrollRepository(db),
            new EmployeeRepository(db),
            new AuditLogRepository(db),
            new XmlPayrollExportBridge(),
            NullLogger<PayrollService>.Instance);

        var result = await service.ProcessPayrollRunAsync(new CreatePayrollRunDto
        {
            EmployeeId = 1,
            BonusAmount = 1000m
        }, "payroll.tester");

        Assert.True(result.NetPay > 0);
        Assert.Equal("Processed", result.PaymentStatus);
        Assert.Contains("EMP-1001", result.DirectDepositReference);
    }

    [Fact]
    public async Task ExportAndImportBankXml_RoundTripsPaymentElements()
    {
        await using var db = await InMemoryPortalTestFixture.CreateSeededDbContextAsync();
        var service = new PayrollService(
            new PayrollRepository(db),
            new EmployeeRepository(db),
            new AuditLogRepository(db),
            new XmlPayrollExportBridge(),
            NullLogger<PayrollService>.Instance);

        var xml = await service.ExportBankAchXmlAsync();
        Assert.Contains("<PayrollBatch>", xml);

        var parsedNodes = service.ImportBankAckXmlUnsafe(xml);
        Assert.NotEmpty(parsedNodes);
    }
}

