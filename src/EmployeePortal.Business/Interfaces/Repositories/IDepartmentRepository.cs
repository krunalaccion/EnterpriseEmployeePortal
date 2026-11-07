using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<IReadOnlyList<Department>> GetDepartmentsWithEmployeesAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

