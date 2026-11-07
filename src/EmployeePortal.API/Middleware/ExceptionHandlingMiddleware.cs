using System.Net;
using Newtonsoft.Json;

namespace EmployeePortal.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing request {Method} {Path}", context.Request.Method, context.Request.Path);

            // =========================================================================
            // [SECURITY-TEST-SCENARIO] ID: VULN-INFO-02 | CWE-209: Generation of Error Message Containing Sensitive Information
            // WARNING: INTENTIONALLY VULNERABLE ERROR RESPONSE FOR SECURESCANAI BENCHMARKING ONLY.
            // Exposes full exception stack trace, inner exception details, and runtime environment metadata.
            // =========================================================================
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var diagnosticPayload = new
            {
                Error = ex.Message,
                ExceptionType = ex.GetType().FullName,
                StackTrace = ex.ToString(),
                InnerException = ex.InnerException?.ToString(),
                RequestPath = context.Request.Path.Value,
                MachineName = Environment.MachineName,
                OsVersion = Environment.OSVersion.ToString(),
                DotNetVersion = Environment.Version.ToString()
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(diagnosticPayload));
        }
    }
}

