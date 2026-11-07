using System.Net;
using System.Net.Http.Json;
using EmployeePortal.Models.DTOs;
using Xunit;

namespace EmployeePortal.IntegrationTests;

public class EmployeeAndAuthEndpointsTests : IClassFixture<PortalApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EmployeeAndAuthEndpointsTests(PortalApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboardSummary_ReturnsSeededMetrics()
    {
        var summary = await _client.GetFromJsonAsync<DashboardSummaryDto>("/api/dashboard");
        Assert.NotNull(summary);
        Assert.True(summary!.TotalEmployees >= 6);
        Assert.Equal(5, summary.TotalDepartments);
    }

    [Fact]
    public async Task Login_WithValidDummyAdminCredentials_ReturnsJwtToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            Username = "admin",
            Password = "Admin@123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(auth);
        Assert.True(auth!.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
    }

    [Fact]
    public async Task SearchEmployees_ExposesSensitiveFields_DemonstratingCwe200()
    {
        var paged = await _client.GetFromJsonAsync<PagedResultDto<EmployeeDto>>("/api/employees?pageNumber=1&pageSize=5");
        Assert.NotNull(paged);
        Assert.NotEmpty(paged!.Items);
        Assert.False(string.IsNullOrWhiteSpace(paged.Items[0].EncryptedTaxId));
        Assert.False(string.IsNullOrWhiteSpace(paged.Items[0].BankRoutingNumber));
    }
}

