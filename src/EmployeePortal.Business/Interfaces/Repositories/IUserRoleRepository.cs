using EmployeePortal.Models.Entities;

namespace EmployeePortal.Business.Interfaces.Repositories;

public interface IUserRoleRepository
{
    Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApplicationUser>> GetAllUsersWithRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task AssignRoleToUserAsync(int userId, string roleName, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}

