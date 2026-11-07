using EmployeePortal.Models.DTOs;

namespace EmployeePortal.Business.Interfaces.Services;

public interface IAuthAndUserService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccountDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignRoleAsync(AssignUserRoleDto dto, string actorUsername, CancellationToken cancellationToken = default);
}

