using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Enums;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IAdminAuditNotificationService
{
    Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(int count = 50, string? moduleFilter = null, CancellationToken cancellationToken = default);
    Task RecordAuditEventAsync(string actorUsername, string actionName, string moduleName, string targetId, string detailsJson, AuditSeverity severity = AuditSeverity.Information, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(int? userId = null, CancellationToken cancellationToken = default);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default);
    Task MarkNotificationReadAsync(int notificationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemSettingDto>> GetSystemSettingsAsync(CancellationToken cancellationToken = default);
    Task<SystemSettingDto> UpsertSystemSettingAsync(UpdateSystemSettingDto dto, string actorUsername, CancellationToken cancellationToken = default);
    Task<string> ExportSystemStateSnapshotAsync(CancellationToken cancellationToken = default);
    object? ImportSystemStateSnapshotUnsafe(string serializedJsonState);
}

