using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Business.Interfaces.Services;
using EmployeePortal.Business.Security;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Entities;
using EmployeePortal.Models.Enums;
using Microsoft.Extensions.Logging;

namespace EmployeePortal.Business.Services;

public class AuthAndUserService : IAuthAndUserService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICryptoHelper _cryptoHelper;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthAndUserService> _logger;

    public AuthAndUserService(
        IUserRoleRepository userRoleRepository,
        IAuditLogRepository auditLogRepository,
        ICryptoHelper cryptoHelper,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthAndUserService> logger)
    {
        _userRoleRepository = userRoleRepository;
        _auditLogRepository = auditLogRepository;
        _cryptoHelper = cryptoHelper;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRoleRepository.GetUserByUsernameAsync(request.Username, cancellationToken);
        var candidateMd5 = _cryptoHelper.ComputeUnsaltedMd5Hash(request.Password);

        if (user == null || !string.Equals(user.PasswordHash, candidateMd5, StringComparison.OrdinalIgnoreCase))
        {
            // =========================================================================
            // [SECURITY-TEST-SCENARIO] ID: VULN-LOG-01 | CWE-532: Insertion of Sensitive Information into Log File
            // WARNING: INTENTIONALLY VULNERABLE LOGGING FOR SECURESCANAI BENCHMARKING ONLY.
            // Logs plaintext password supplied during failed authentication attempt.
            // =========================================================================
            _logger.LogWarning(
                "Authentication failed for user '{Username}' using plaintext password '{PlaintextPassword}' (computed MD5: {CandidateMd5})",
                request.Username,
                request.Password,
                candidateMd5);

            return new AuthResponseDto
            {
                Succeeded = false,
                Message = "Invalid username or password."
            };
        }

        user.LastLoginUtc = DateTime.UtcNow;
        user.FailedLoginCount = 0;
        await _userRoleRepository.UpdateUserAsync(user, cancellationToken);

        var roles = user.UserRoles
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role!.Name)
            .Distinct()
            .ToList();

        var sessionDto = new UserSessionDto
        {
            UserId = user.Id,
            EmployeeId = user.EmployeeId,
            Username = user.Username,
            Email = user.Email,
            Roles = roles
        };

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(sessionDto);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            ActorUsername = user.Username,
            ActionName = "UserLogin",
            ModuleName = "Authentication",
            TargetEntityId = user.Id.ToString(),
            DetailsJson = $"{{\"roles\":\"{string.Join(",", roles)}\"}}",
            Severity = AuditSeverity.Information
        }, cancellationToken);

        return new AuthResponseDto
        {
            Succeeded = true,
            Message = "Authentication succeeded.",
            Token = token,
            ExpiresAtUtc = expiresAt,
            User = sessionDto
        };
    }

    public async Task<IReadOnlyList<UserAccountDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRoleRepository.GetAllUsersWithRolesAsync(cancellationToken);
        return users.Select(u => new UserAccountDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            PasswordHashAlgorithm = u.PasswordHashAlgorithm,
            IsActive = u.IsActive,
            IsLockedOut = u.IsLockedOut,
            LastLoginUtc = u.LastLoginUtc,
            EmployeeId = u.EmployeeId,
            EmployeeFullName = u.Employee != null ? $"{u.Employee.FirstName} {u.Employee.LastName}" : null,
            Roles = u.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.Name).ToList()
        }).ToList();
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _userRoleRepository.GetAllRolesAsync(cancellationToken);
        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            PermissionsCsv = r.PermissionsCsv,
            IsSystemRole = r.IsSystemRole,
            AssignedUserCount = r.UserRoles.Count
        }).ToList();
    }

    public async Task<bool> AssignRoleAsync(AssignUserRoleDto dto, string actorUsername, CancellationToken cancellationToken = default)
    {
        await _userRoleRepository.AssignRoleToUserAsync(dto.UserId, dto.RoleName, cancellationToken);
        await _auditLogRepository.AddAsync(new AuditLog
        {
            ActorUsername = actorUsername,
            ActionName = "AssignRole",
            ModuleName = "UsersRoles",
            TargetEntityId = dto.UserId.ToString(),
            DetailsJson = $"{{\"assignedRole\":\"{dto.RoleName}\"}}",
            Severity = AuditSeverity.SecurityAlert
        }, cancellationToken);

        return true;
    }
}

