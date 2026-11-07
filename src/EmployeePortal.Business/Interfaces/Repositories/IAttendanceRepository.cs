using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IAttendanceRepository : IRepository<AttendanceRecord>
{
    Task<IReadOnlyList<AttendanceRecord>> GetAttendanceRecordsWithEmployeeAsync(int? employeeId = null, CancellationToken cancellationToken = default);
}

