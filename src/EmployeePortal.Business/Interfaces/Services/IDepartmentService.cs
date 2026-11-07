using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<DepartmentDto?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DepartmentDto> CreateDepartmentAsync(UpsertDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpsertDepartmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteDepartmentAsync(int id, CancellationToken cancellationToken = default);
}

