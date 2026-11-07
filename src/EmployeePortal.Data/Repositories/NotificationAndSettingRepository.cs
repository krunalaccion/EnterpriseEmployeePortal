using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class NotificationAndSettingRepository : INotificationAndSettingRepository
{
    private readonly PortalDbContext _dbContext;

    public NotificationAndSettingRepository(PortalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Notification>> GetNotificationsAsync(int? recipientUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Notifications.AsNoTracking().AsQueryable();

        if (recipientUserId.HasValue && recipientUserId.Value > 0)
        {
            query = query.Where(n => n.IsBroadcast || n.RecipientUserId == recipientUserId.Value);
        }

        return await query
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification> AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return notification;
    }

    public async Task MarkNotificationAsReadAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);
        if (existing != null)
        {
            existing.IsRead = true;
            existing.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<SystemSetting>> GetAllSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SystemSettings
            .AsNoTracking()
            .OrderBy(s => s.GroupName)
            .ThenBy(s => s.SettingKey)
            .ToListAsync(cancellationToken);
    }

    public async Task<SystemSetting> UpsertSettingAsync(SystemSetting setting, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingKey.ToLower() == setting.SettingKey.ToLower(), cancellationToken);

        if (existing == null)
        {
            await _dbContext.SystemSettings.AddAsync(setting, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return setting;
        }

        existing.SettingValue = setting.SettingValue;
        existing.GroupName = setting.GroupName;
        existing.Description = setting.Description;
        existing.IsSensitive = setting.IsSensitive;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }
}

