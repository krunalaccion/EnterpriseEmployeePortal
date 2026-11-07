using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface INotificationAndSettingRepository
{
    Task<IReadOnlyList<Notification>> GetNotificationsAsync(int? recipientUserId = null, CancellationToken cancellationToken = default);
    Task<Notification> AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
    Task MarkNotificationAsReadAsync(int notificationId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemSetting>> GetAllSettingsAsync(CancellationToken cancellationToken = default);
    Task<SystemSetting> UpsertSettingAsync(SystemSetting setting, CancellationToken cancellationToken = default);
}

