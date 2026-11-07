using System.Text.Encodings.Web;
using EmployeePortal.Business.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EmployeePortal.API.Security;

public class PortalJwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "PortalBearer";
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public PortalJwtAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IJwtTokenGenerator jwtTokenGenerator)
        : base(options, logger, encoder)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var headerValue = authHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(headerValue) || !headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var rawToken = headerValue["Bearer ".Length..].Trim();
        var principal = _jwtTokenGenerator.ValidateToken(rawToken);
        if (principal == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid or expired JWT token."));
        }

        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

