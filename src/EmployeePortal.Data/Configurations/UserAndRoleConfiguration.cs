using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeePortal.Data.Configurations;

public class UserAndRoleConfiguration :
    IEntityTypeConfiguration<ApplicationUser>,
    IEntityTypeConfiguration<Role>,
    IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).HasMaxLength(80).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(150).IsRequired();

        builder.HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(64).IsRequired();
    }

    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(ur => ur.Id);

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

