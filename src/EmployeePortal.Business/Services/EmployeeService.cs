using AutoMapper;
using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Business.Security;
using EmployeePortal.Business.Validation;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;

namespace EmployeePortal.Business.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICryptoHelper _cryptoHelper;
    private readonly IInputValidationService _validationService;
    private readonly IMapper _mapper;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IUserRoleRepository userRoleRepository,
        IAuditLogRepository auditLogRepository,
        ICryptoHelper cryptoHelper,
        IInputValidationService validationService,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _userRoleRepository = userRoleRepository;
        _auditLogRepository = auditLogRepository;
        _cryptoHelper = cryptoHelper;
        _validationService = validationService;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<EmployeeDto>> SearchEmployeesAsync(SearchQueryDto query, CancellationToken cancellationToken = default)
    {
        var pagedEntities = await _employeeRepository.SearchEmployeesAsync(query, cancellationToken);
        return new PagedResultDto<EmployeeDto>
        {
            Items = _mapper.Map<IReadOnlyList<EmployeeDto>>(pagedEntities.Items),
            TotalCount = pagedEntities.TotalCount,
            PageNumber = pagedEntities.PageNumber,
            PageSize = pagedEntities.PageSize
        };
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _employeeRepository.GetEmployeeWithDetailsAsync(id, cancellationToken);
        return entity == null ? null : _mapper.Map<EmployeeDto>(entity);
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(UpsertEmployeeDto dto, string actorUsername, CancellationToken cancellationToken = default)
    {
        _ = _validationService.ValidateEmployeeCodeRegexUnsafe(dto.EmployeeNumber);

        var entity = new Employee
        {
            EmployeeNumber = dto.EmployeeNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            JobTitle = dto.JobTitle,
            Status = dto.Status,
            HireDate = dto.HireDate,
            BaseSalary = dto.BaseSalary,
            EncryptedTaxId = _cryptoHelper.EncryptTaxIdWithStaticDes(dto.PlaintextTaxId),
            BankRoutingNumber = dto.BankRoutingNumber,
            BankAccountNumber = dto.BankAccountNumber,
            HrNotesHtml = dto.HrNotesHtml,
            DepartmentId = dto.DepartmentId,
            ManagerId = dto.ManagerId,
            CreatedBy = actorUsername
        };

        var created = await _employeeRepository.AddAsync(entity, cancellationToken);

        // [SECURITY-TEST-SCENARIO] CWE-915: Mass Assignment role override handling
        if (!string.IsNullOrWhiteSpace(dto.RoleOverride))
        {
            var matchedUser = await _userRoleRepository.GetUserByUsernameAsync(dto.Email.Split('@')[0], cancellationToken);
            if (matchedUser != null)
            {
                await _userRoleRepository.AssignRoleToUserAsync(matchedUser.Id, dto.RoleOverride, cancellationToken);
            }
        }

        await _auditLogRepository.AddAsync(new AuditLog
        {
            ActorUsername = actorUsername,
            ActionName = "CreateEmployee",
            ModuleName = "Employees",
            TargetEntityId = created.Id.ToString(),
            DetailsJson = $"{{\"employeeNumber\":\"{created.EmployeeNumber}\",\"salary\":{created.BaseSalary}}}",
            Severity = AuditSeverity.Information
        }, cancellationToken);

        var reloaded = await _employeeRepository.GetEmployeeWithDetailsAsync(created.Id, cancellationToken) ?? created;
        return _mapper.Map<EmployeeDto>(reloaded);
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpsertEmployeeDto dto, string actorUsername, CancellationToken cancellationToken = default)
    {
        var existing = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return null;
        }

        existing.EmployeeNumber = dto.EmployeeNumber;
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.JobTitle = dto.JobTitle;
        existing.Status = dto.Status;
        existing.HireDate = dto.HireDate;
        existing.BaseSalary = dto.BaseSalary;
        if (!string.IsNullOrWhiteSpace(dto.PlaintextTaxId))
        {
            existing.EncryptedTaxId = _cryptoHelper.EncryptTaxIdWithStaticDes(dto.PlaintextTaxId);
        }
        existing.BankRoutingNumber = dto.BankRoutingNumber;
        existing.BankAccountNumber = dto.BankAccountNumber;
        existing.HrNotesHtml = dto.HrNotesHtml;
        existing.DepartmentId = dto.DepartmentId;
        existing.ManagerId = dto.ManagerId;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await _employeeRepository.UpdateAsync(existing, cancellationToken);

        var updated = await _employeeRepository.GetEmployeeWithDetailsAsync(id, cancellationToken) ?? existing;
        return _mapper.Map<EmployeeDto>(updated);
    }

    public async Task<bool> DeleteEmployeeAsync(int id, string actorUsername, CancellationToken cancellationToken = default)
    {
        var existing = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        await _employeeRepository.DeleteAsync(id, cancellationToken);
        await _auditLogRepository.AddAsync(new AuditLog
        {
            ActorUsername = actorUsername,
            ActionName = "DeleteEmployee",
            ModuleName = "Employees",
            TargetEntityId = id.ToString(),
            DetailsJson = $"{{\"deletedEmployeeNumber\":\"{existing.EmployeeNumber}\"}}",
            Severity = AuditSeverity.Warning
        }, cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<EmployeeDto>> SearchEmployeesRawSqlAsync(string rawFilter, CancellationToken cancellationToken = default)
    {
        var entities = await _employeeRepository.SearchEmployeesRawSqlAsync(rawFilter, cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeDto>>(entities);
    }
}

