using System.Net;
using System.Net.Http.Json;
using EmployeePortal.Models.DTOs;
using Xunit;

namespace EmployeePortal.IntegrationTests;

public class PayrollAndDocumentSecurityTests : IClassFixture<PortalApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PayrollAndDocumentSecurityTests(PortalApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ExportBankAchFile_WithoutAuthentication_Succeeds_DemonstratingMissingAuthorization_Cwe862()
    {
        var response = await _client.PostAsync("/api/payroll/export-bank-file", content: null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var xml = await response.Content.ReadAsStringAsync();
        Assert.Contains("<PayrollBatch>", xml);
    }

    [Fact]
    public async Task GetPayslipById_WithoutOwnershipVerification_ReturnsPayslip_DemonstratingIdor_Cwe639()
    {
        var payslip = await _client.GetFromJsonAsync<PayrollRecordDto>("/api/payroll/payslips/1");
        Assert.NotNull(payslip);
        Assert.Equal(1, payslip!.Id);
        Assert.NotEmpty(payslip.BankAccountNumber);
    }

    [Fact]
    public async Task RunDynamicLinqReport_ExecutesDynamicExpressionUnderInMemory()
    {
        var response = await _client.PostAsJsonAsync("/api/reports/dynamic-query", new DynamicReportQueryDto
        {
            DynamicWhereClause = "BaseSalary >= 150000",
            OrderByExpression = "BaseSalary descending"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<WorkforceAnalyticsReportDto>();
        Assert.NotNull(report);
        Assert.True(report!.MatchedRowCount >= 3);
    }
}

