using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class LeaveRepository : GenericRepository<LeaveRequest>, ILeaveRepository
{
    public LeaveRepository(PortalDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbContext.LeaveRequests
            .AsNoTracking()
            .Include(l => l.Employee)
            .ThenInclude(e => e!.Department)
            .AsQueryable();

        if (employeeId.HasValue && employeeId.Value > 0)
        {
            query = query.Where(l => l.EmployeeId == employeeId.Value);
        }

        return await query
            .OrderByDescending(l => l.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveAllocation>> GetAllocationsByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await DbContext.LeaveAllocations
            .AsNoTracking()
            .Where(a => a.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }
}

