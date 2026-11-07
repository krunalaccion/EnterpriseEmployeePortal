using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeePortal.Data.Configurations;

public class DocumentAndReportConfiguration :
    IEntityTypeConfiguration<EmployeeDocument>,
    IEntityTypeConfiguration<ReportTemplate>
{
    public void Configure(EntityTypeBuilder<EmployeeDocument> builder)
    {
        builder.ToTable("EmployeeDocuments");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Title).HasMaxLength(160).IsRequired();
        builder.Property(d => d.FileName).HasMaxLength(255).IsRequired();

        builder.HasOne(d => d.Employee)
            .WithMany(e => e.Documents)
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<ReportTemplate> builder)
    {
        builder.ToTable("ReportTemplates");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(140).IsRequired();
    }
}

