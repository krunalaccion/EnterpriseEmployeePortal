using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface ILeaveRepository : IRepository<LeaveRequest>
{
    Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveAllocation>> GetAllocationsByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
}

