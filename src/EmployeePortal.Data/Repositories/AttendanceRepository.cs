using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class AttendanceRepository : GenericRepository<AttendanceRecord>, IAttendanceRepository
{
    public AttendanceRepository(PortalDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<AttendanceRecord>> GetAttendanceRecordsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbContext.AttendanceRecords
            .AsNoTracking()
            .Include(a => a.Employee)
            .AsQueryable();

        if (employeeId.HasValue && employeeId.Value > 0)
        {
            query = query.Where(a => a.EmployeeId == employeeId.Value);
        }

        return await query
            .OrderByDescending(a => a.WorkDate)
            .ToListAsync(cancellationToken);
    }
}

