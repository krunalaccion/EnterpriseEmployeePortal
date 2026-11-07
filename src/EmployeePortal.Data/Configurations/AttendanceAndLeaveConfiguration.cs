using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeePortal.Data.Configurations;

public class AttendanceAndLeaveConfiguration :
    IEntityTypeConfiguration<AttendanceRecord>,
    IEntityTypeConfiguration<LeaveRequest>,
    IEntityTypeConfiguration<LeaveAllocation>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.RegularHours).HasPrecision(8, 2);
        builder.Property(a => a.OvertimeHours).HasPrecision(8, 2);

        builder.HasOne(a => a.Employee)
            .WithMany(e => e.AttendanceRecords)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.TotalDays).HasPrecision(8, 2);

        builder.HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<LeaveAllocation> builder)
    {
        builder.ToTable("LeaveAllocations");
        builder.HasKey(la => la.Id);
        builder.Property(la => la.AllocatedDays).HasPrecision(8, 2);
        builder.Property(la => la.UsedDays).HasPrecision(8, 2);

        builder.HasOne(la => la.Employee)
            .WithMany(e => e.LeaveAllocations)
            .HasForeignKey(la => la.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

