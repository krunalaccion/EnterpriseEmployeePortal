using EmployeePortal.Models.Enums;

namespace EmployeePortal.Models.Entities;

public class AuditLog : BaseEntity
{
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string ActorUsername { get; set; } = "anonymous";
    public string ActionName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string TargetEntityId { get; set; } = string.Empty;
    public string ClientIpAddress { get; set; } = "127.0.0.1";
    public string DetailsJson { get; set; } = "{}";
    public AuditSeverity Severity { get; set; } = AuditSeverity.Information;
}

