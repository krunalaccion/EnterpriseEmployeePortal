using EmployeePortal.Web.Components;
using EmployeePortal.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<PortalUserSessionService>();

var apiBaseUrl = builder.Configuration["PortalApi:BaseUrl"] ?? "http://localhost:5180";
builder.Services.AddHttpClient<IPortalApiClient, PortalApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.EndsWith('/') ? apiBaseUrl : $"{apiBaseUrl}/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    app.UseHsts();}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

