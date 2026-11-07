using AutoMapper;
using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Integrations.Messaging;
using EmployeePortal.Integrations.Serialization;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;

namespace EmployeePortal.Business.Services;

public class AdminAuditNotificationService : IAdminAuditNotificationService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly INotificationAndSettingRepository _notificationAndSettingRepository;
    private readonly IEmailService _emailService;
    private readonly IStateSerializer _stateSerializer;
    private readonly IMapper _mapper;

    public AdminAuditNotificationService(
        IAuditLogRepository auditLogRepository,
        INotificationAndSettingRepository notificationAndSettingRepository,
        IEmailService emailService,
        IStateSerializer stateSerializer,
        IMapper mapper)
    {
        _auditLogRepository = auditLogRepository;
        _notificationAndSettingRepository = notificationAndSettingRepository;
        _emailService = emailService;
        _stateSerializer = stateSerializer;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(int count = 50, string? moduleFilter = null, CancellationToken cancellationToken = default)
    {
        var logs = await _auditLogRepository.GetRecentLogsAsync(count, moduleFilter, cancellationToken);
        return _mapper.Map<IReadOnlyList<AuditLogDto>>(logs);
    }

    public async Task RecordAuditEventAsync(
        string actorUsername,
        string actionName,
        string moduleName,
        string targetId,
        string detailsJson,
        AuditSeverity severity = AuditSeverity.Information,
        CancellationToken cancellationToken = default)
    {
        await _auditLogRepository.AddAsync(new AuditLog
        {
            TimestampUtc = DateTime.UtcNow,
            ActorUsername = actorUsername,
            ActionName = actionName,
            ModuleName = moduleName,
            TargetEntityId = targetId,
            DetailsJson = detailsJson,
            Severity = severity
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationAndSettingRepository.GetNotificationsAsync(userId, cancellationToken);
        return _mapper.Map<IReadOnlyList<NotificationDto>>(notifications);
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Notification
        {
            RecipientUserId = dto.RecipientUserId,
            Title = dto.Title,
            MessageHtml = dto.MessageHtml,
            Category = dto.Category,
            IsBroadcast = dto.IsBroadcast,
            IsRead = false
        };

        var saved = await _notificationAndSettingRepository.AddNotificationAsync(entity, cancellationToken);
        await _emailService.SendNotificationEmailAsync("all-staff@dummy-hr-portal.local", dto.Title, dto.MessageHtml, cancellationToken);
        return _mapper.Map<NotificationDto>(saved);
    }

    public async Task MarkNotificationReadAsync(int notificationId, CancellationToken cancellationToken = default)
    {
        await _notificationAndSettingRepository.MarkNotificationAsReadAsync(notificationId, cancellationToken);
    }

    public async Task<IReadOnlyList<SystemSettingDto>> GetSystemSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _notificationAndSettingRepository.GetAllSettingsAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SystemSettingDto>>(settings);
    }

    public async Task<SystemSettingDto> UpsertSystemSettingAsync(UpdateSystemSettingDto dto, string actorUsername, CancellationToken cancellationToken = default)
    {
        var entity = new SystemSetting
        {
            SettingKey = dto.SettingKey,
            SettingValue = dto.SettingValue,
            GroupName = dto.GroupName,
            Description = dto.Description,
            IsSensitive = dto.IsSensitive,
            CreatedBy = actorUsername
        };

        var saved = await _notificationAndSettingRepository.UpsertSettingAsync(entity, cancellationToken);
        await RecordAuditEventAsync(
            actorUsername,
            "UpsertSystemSetting",
            "Administration",
            saved.SettingKey,
            $"{{\"key\":\"{saved.SettingKey}\",\"group\":\"{saved.GroupName}\"}}",
            AuditSeverity.SecurityAlert,
            cancellationToken);

        return _mapper.Map<SystemSettingDto>(saved);
    }

    public async Task<string> ExportSystemStateSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var settings = await GetSystemSettingsAsync(cancellationToken);
        return _stateSerializer.SerializeStateSnapshot(settings);
    }

    public object? ImportSystemStateSnapshotUnsafe(string serializedJsonState)
    {
        return _stateSerializer.DeserializeStateSnapshotUnsafe(serializedJsonState);
    }
}

