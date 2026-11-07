using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeePortal.Data.Configurations;

public class AuditNotificationSettingConfiguration :
    IEntityTypeConfiguration<AuditLog>,
    IEntityTypeConfiguration<Notification>,
    IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.ActorUsername).HasMaxLength(100);
        builder.Property(a => a.ActionName).HasMaxLength(120);
        builder.Property(a => a.ModuleName).HasMaxLength(80);
    }

    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).HasMaxLength(180).IsRequired();
    }

    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.SettingKey).HasMaxLength(120).IsRequired();
    }
}

