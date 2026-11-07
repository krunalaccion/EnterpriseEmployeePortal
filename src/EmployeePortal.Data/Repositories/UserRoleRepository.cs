using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly PortalDbContext _dbContext;

    public UserRoleRepository(PortalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Employee)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower(), cancellationToken);
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.Employee)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetAllUsersWithRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Employee)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .AsNoTracking()
            .Include(r => r.UserRoles)
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignRoleToUserAsync(int userId, string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower(), cancellationToken);
        if (role == null)
        {
            role = new Role
            {
                Name = roleName,
                Description = $"Dynamically created role ({roleName})",
                PermissionsCsv = "Custom",
                IsSystemRole = false
            };
            await _dbContext.Roles.AddAsync(role, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var exists = await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == role.Id, cancellationToken);

        if (!exists)
        {
            await _dbContext.UserRoles.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                AssignedAtUtc = DateTime.UtcNow
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateUserAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

