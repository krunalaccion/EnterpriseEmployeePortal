using AutoMapper;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Mapping;

/// <summary>
/// AutoMapper profile exercising AutoMapper 9.0.0 (SCA benchmark for CVE-2026-32933 / GHSA-rvv3-g6hj-g44x).
/// Intentionally omits MaxDepth() to reflect the advisory's uncontrolled recursion pattern on self-referencing entities.
/// </summary>
public class PortalMappingProfile : Profile
{
    public PortalMappingProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : "Unassigned"))
            .ForMember(d => d.ManagerName, opt => opt.MapFrom(s => s.Manager != null ? $"{s.Manager.FirstName} {s.Manager.LastName}" : null));

        CreateMap<Department, DepartmentDto>()
            .ForMember(d => d.EmployeeCount, opt => opt.MapFrom(s => s.Employees != null ? s.Employees.Count : 0));

        CreateMap<ReportTemplate, ReportTemplateDto>();
        CreateMap<AuditLog, AuditLogDto>();
        CreateMap<Notification, NotificationDto>();
        CreateMap<SystemSetting, SystemSettingDto>();
    }
}

