using System.Linq.Expressions;
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
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.Set<T>().ToListAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<T?> Get(Expression<Func<T, bool>> func, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.Set<T>().FirstOrDefaultAsync(func, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetById(Guid id, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.Set<T>().FindAsync(id, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IfExists(Func<T, bool> func, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var entities = context.Set<T>().Where(func);
            return entities.Any();
        }
    }


    /// <inheritdoc/>
    public async Task Add(T entity, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            entity.CreateDate = DateTime.UtcNow;
            
            await context.Set<T>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Update(T entity, CancellationToken cancellationToken)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
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
    public async Task<bool> Delete(T entity, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
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
    public async Task<bool> HardDelete(Guid id, CancellationToken cancellationToken)
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
    public async Task<bool> HardDelete(T? entity, CancellationToken cancellationToken)
    {
        if (entity is null)
        {
            return false;
        }

        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    /// <inheritdoc/>
    public async Task<ICollection<T>> Where(Func<T, bool> func, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return context.Set<T>().Where(func).ToArray();
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetLast(Expression<Func<T, bool>> func, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await context.Set<T>()
                .OrderByDescending(s => s.CreateDate).LastOrDefaultAsync(func, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Save(CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
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