using EmployeePortal.API.Middleware;
using EmployeePortal.API.Security;
using EmployeePortal.Business;
using EmployeePortal.Data;
using EmployeePortal.Data.Context;
using EmployeePortal.Integrations;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Composition Root: Register Data, Integrations, and Business layers explicitly
builder.Services.AddPortalData(builder.Configuration);
builder.Services.AddPortalIntegrations(builder.Configuration);
builder.Services.AddPortalBusiness(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddAuthentication(PortalJwtAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, PortalJwtAuthenticationHandler>(
        PortalJwtAuthenticationHandler.SchemeName,
        _ => { });

builder.Services.AddAuthorization();

// =========================================================================
// [SECURITY-TEST-SCENARIO] ID: VULN-CORS-01 | CWE-942: Permissive Cross-domain Policy with Untrusted Domains
// WARNING: INTENTIONALLY VULNERABLE CORS CONFIGURATION FOR SECURESCANAI BENCHMARKING ONLY.
// Reflects arbitrary Origin headers while allowing credentials, headers, and methods.
// =========================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermissiveScannerTestCors", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
    var provider = builder.Configuration["DatabaseOptions:Provider"] ?? "SqlServer";
    if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
    {
        await PortalDbSeeder.SeedAsync(dbContext);
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<AuditRequestLoggingMiddleware>();

app.UseCors("PermissiveScannerTestCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

