using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public DateTime TimestampUtc { get; set; }
    public string ActorUsername { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string TargetEntityId { get; set; } = string.Empty;
    public string ClientIpAddress { get; set; } = string.Empty;
    public string DetailsJson { get; set; } = string.Empty;
    public AuditSeverity Severity { get; set; }
}

public class NotificationDto
{
    public int Id { get; set; }
    public int? RecipientUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MessageHtml { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsBroadcast { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreateNotificationDto
{
    public int? RecipientUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MessageHtml { get; set; } = string.Empty;
    public string Category { get; set; } = "Announcement";
    public bool IsBroadcast { get; set; } = true;
}

