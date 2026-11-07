using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IReadOnlyList<AuditLog>> GetRecentLogsAsync(int count = 50, string? moduleFilter = null, CancellationToken cancellationToken = default);
}

