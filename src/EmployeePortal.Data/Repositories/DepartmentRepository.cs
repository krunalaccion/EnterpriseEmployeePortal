using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(PortalDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Department>> GetDepartmentsWithEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Departments
            .AsNoTracking()
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbContext.Departments
            .FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }
}

