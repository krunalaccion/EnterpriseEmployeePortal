using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class PayrollRepository : GenericRepository<PayrollRecord>, IPayrollRepository
{
    public PayrollRepository(PortalDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<PayrollRecord>> GetPayrollRecordsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbContext.PayrollRecords
            .AsNoTracking()
            .Include(p => p.Employee)
            .ThenInclude(e => e!.Department)
            .AsQueryable();

        if (employeeId.HasValue && employeeId.Value > 0)
        {
            query = query.Where(p => p.EmployeeId == employeeId.Value);
        }

        return await query
            .OrderByDescending(p => p.PayPeriodEnd)
            .ToListAsync(cancellationToken);
    }

    public async Task<PayrollRecord?> GetPayslipByIdWithEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.PayrollRecords
            .AsNoTracking()
            .Include(p => p.Employee)
            .ThenInclude(e => e!.Department)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}

