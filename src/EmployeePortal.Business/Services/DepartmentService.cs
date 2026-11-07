using AutoMapper;
using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _departmentRepository.GetDepartmentsWithEmployeesAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dept = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        return dept == null ? null : _mapper.Map<DepartmentDto>(dept);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(UpsertDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Department
        {
            Name = dto.Name,
            Code = dto.Code.ToUpperInvariant(),
            CostCenter = dto.CostCenter,
            Location = dto.Location,
            AnnualBudget = dto.AnnualBudget,
            ManagerEmployeeId = dto.ManagerEmployeeId,
            IsActive = dto.IsActive
        };

        var created = await _departmentRepository.AddAsync(entity, cancellationToken);
        return _mapper.Map<DepartmentDto>(created);
    }

    public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpsertDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return null;
        }

        existing.Name = dto.Name;
        existing.Code = dto.Code.ToUpperInvariant();
        existing.CostCenter = dto.CostCenter;
        existing.Location = dto.Location;
        existing.AnnualBudget = dto.AnnualBudget;
        existing.ManagerEmployeeId = dto.ManagerEmployeeId;
        existing.IsActive = dto.IsActive;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _departmentRepository.UpdateAsync(existing, cancellationToken);
        return _mapper.Map<DepartmentDto>(existing);
    }

    public async Task<bool> DeleteDepartmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _departmentRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        await _departmentRepository.DeleteAsync(id, cancellationToken);
        return true;
    }
}

