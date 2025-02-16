using DogLiveBot.Data.Context;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DogLiveBot.Data.Repository.RepositoryImplementations;

public class ApplicationRepository<T> : IRepository<T>, IAsyncDisposable where T : BaseEntity<Guid>
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ApplicationRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.Set<T>().ToListAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public T? GetById(Guid id)
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            return context.Set<T>().Find(id);
        }
    }

    /// <inheritdoc/>
    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.Set<T>().AddAsync(entity, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        entity.ModifiedDate = DateTime.Now;
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.Set<T>().FindAsync(id, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var entity = await context.Set<T>().FindAsync(id, cancellationToken);
            if (entity is null)
            {
                return false;
            }

            entity.DeleteDate = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var findAsync = await context.Set<T>().FindAsync(entity.Id, cancellationToken);
            if (findAsync is null)
            {
                return false;
            }

            findAsync.DeleteDate = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    /// <inheritdoc/>
    public bool Delete(ICollection<T> entities)
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            var findEntities = context.Set<T>().Where(t => entities.Contains(t));
            foreach (var item in findEntities)
            {
                item.DeleteDate = DateTime.Now;
            }

            context.SaveChanges();

            return true;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var res = await context.Set<T>().FindAsync(id, cancellationToken);
            if (res == null)
            {
                return false;
            }

            context.Remove(res);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HardDeleteAsync(T? entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            return false;
        }

        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    /// <inheritdoc/>
    public bool HardDelete(ICollection<T>? entities)
    {
        if (entities == null || !entities.Any())
        {
            return false;
        }

        using (var context = _contextFactory.CreateDbContext())
        {
            context.Set<T>().RemoveRange(entities);
            context.SaveChanges();

            return true;
        }
    }

    /// <inheritdoc/>
    public ICollection<T> Where(Func<T, bool> func)
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            return context.Set<T>().Where(func).ToArray();
        }
    }

    /// <inheritdoc/>
    public void Save()
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            context.SaveChanges();
        }
    }

    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
    }
}