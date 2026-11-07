namespace EmployeePortal.Integrations.Messaging;

public interface IEmailService
{
    Task<bool> SendNotificationEmailAsync(string recipientEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

