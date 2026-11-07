using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeePortal.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EmployeeNumber).HasMaxLength(32).IsRequired();
        builder.Property(e => e.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(80).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(150).IsRequired();
        builder.Property(e => e.JobTitle).HasMaxLength(120).IsRequired();
        builder.Property(e => e.BaseSalary).HasPrecision(18, 2);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Manager)
            .WithMany(m => m.DirectReports)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

