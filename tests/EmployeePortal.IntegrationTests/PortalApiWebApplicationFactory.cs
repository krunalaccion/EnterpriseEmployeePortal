using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EmployeePortal.IntegrationTests;

public class PortalApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _inMemoryDbName = $"PortalIntegrationTestDb_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["DatabaseOptions:Provider"] = "InMemory",
                ["DatabaseOptions:InMemoryDatabaseName"] = _inMemoryDbName,
                ["DatabaseOptions:EnableAutoSeed"] = "true"
            };
            configBuilder.AddInMemoryCollection(overrides);
        });
    }
}

