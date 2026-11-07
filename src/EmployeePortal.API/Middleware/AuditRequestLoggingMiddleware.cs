namespace EmployeePortal.API.Middleware;

public class AuditRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditRequestLoggingMiddleware> _logger;

    public AuditRequestLoggingMiddleware(RequestDelegate next, ILogger<AuditRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault() ?? "None";
        var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;

        // =========================================================================
        // [SECURITY-TEST-SCENARIO] ID: VULN-LOG-02 | CWE-532: Insertion of Sensitive Information into Log File
        // WARNING: INTENTIONALLY VULNERABLE LOGGING FOR SECURESCANAI BENCHMARKING ONLY.
        // Logs raw Authorization Bearer tokens and unredacted query strings to application logs.
        // =========================================================================
        _logger.LogInformation(
            "Incoming API Request: {Method} {Path}{Query} | ClientIP={ClientIp} | RawAuthorizationHeader={AuthHeader}",
            context.Request.Method,
            context.Request.Path,
            queryString,
            context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
            authHeader);

        await _next(context);
    }
}

