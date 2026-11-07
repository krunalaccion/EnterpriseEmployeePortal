using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeePortal.Data.Configurations;

public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
{
    public void Configure(EntityTypeBuilder<PayrollRecord> builder)
    {
        builder.ToTable("PayrollRecords");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.GrossSalary).HasPrecision(18, 2);
        builder.Property(p => p.FederalTaxWithheld).HasPrecision(18, 2);
        builder.Property(p => p.StateTaxWithheld).HasPrecision(18, 2);
        builder.Property(p => p.BenefitsDeduction).HasPrecision(18, 2);
        builder.Property(p => p.BonusAmount).HasPrecision(18, 2);
        builder.Property(p => p.NetPay).HasPrecision(18, 2);

        builder.HasOne(p => p.Employee)
            .WithMany(e => e.PayrollRecords)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

