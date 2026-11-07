using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class DocumentRepository : GenericRepository<EmployeeDocument>, IDocumentRepository
{
    public DocumentRepository(PortalDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<EmployeeDocument>> GetDocumentsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbContext.EmployeeDocuments
            .AsNoTracking()
            .Include(d => d.Employee)
            .AsQueryable();

        if (employeeId.HasValue && employeeId.Value > 0)
        {
            query = query.Where(d => d.EmployeeId == employeeId.Value);
        }

        return await query
            .OrderByDescending(d => d.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}

