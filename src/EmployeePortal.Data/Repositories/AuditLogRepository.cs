using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(PortalDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<AuditLog>> GetRecentLogsAsync(int count = 50, string? moduleFilter = null, CancellationToken cancellationToken = default)
    {
        var query = DbContext.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(moduleFilter))
        {
            query = query.Where(a => a.ModuleName.ToLower() == moduleFilter.ToLower());
        }

        return await query
            .OrderByDescending(a => a.TimestampUtc)
            .Take(Math.Clamp(count, 1, 500))
            .ToListAsync(cancellationToken);
    }
}

