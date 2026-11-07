using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeePortal.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).HasMaxLength(120).IsRequired();
        builder.Property(d => d.Code).HasMaxLength(24).IsRequired();
        builder.Property(d => d.CostCenter).HasMaxLength(32);
        builder.Property(d => d.Location).HasMaxLength(100);
        builder.Property(d => d.AnnualBudget).HasPrecision(18, 2);
    }
}

