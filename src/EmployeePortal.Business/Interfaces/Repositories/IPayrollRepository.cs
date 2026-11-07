using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IPayrollRepository : IRepository<PayrollRecord>
{
    Task<IReadOnlyList<PayrollRecord>> GetPayrollRecordsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<PayrollRecord?> GetPayslipByIdWithEmployeeAsync(int id, CancellationToken cancellationToken = default);
}

