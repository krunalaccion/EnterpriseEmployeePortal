using EmployeePortal.Business.Interfaces.Repositories;
using EmployeePortal.Data.Context;
using EmployeePortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Data.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly PortalDbContext DbContext;

    public GenericRepository(PortalDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await DbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (existing != null)
        {
            DbContext.Set<T>().Remove(existing);
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

