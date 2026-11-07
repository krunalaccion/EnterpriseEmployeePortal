using EmployeePortal.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmployeePortal.Integrations.Messaging;

public class SmtpEmailService : IEmailService
{
    // =========================================================================
    // [SECURITY-TEST-SCENARIO] ID: VULN-SECRET-02 | CWE-798: Use of Hard-coded Credentials
    // WARNING: DUMMY CREDENTIALS FOR SECURESCANAI SAST TESTING ONLY. NOT A REAL SECRET.
    // =========================================================================
    private const string DummyFallbackSmtpApiKey = "SG.DUMMY_HARDCODED_SMTP_KEY_00000000000000000000000000";
    private readonly IntegrationSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<IntegrationSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task<bool> SendNotificationEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var activeCredential = string.IsNullOrWhiteSpace(_settings.SmtpPassword)
            ? DummyFallbackSmtpApiKey
            : _settings.SmtpPassword;

        _logger.LogInformation(
            "Simulated SMTP dispatch to {Recipient} via {Host}:{Port} (Auth token length: {Length}) Subject: {Subject}",
            recipientEmail,
            _settings.SmtpHost,
            _settings.SmtpPort,
            activeCredential.Length,
            subject);

        return Task.FromResult(true);
    }
}

