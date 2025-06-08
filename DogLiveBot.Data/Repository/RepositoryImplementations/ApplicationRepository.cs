using System.Linq.Expressions;
using DogLiveBot.Data.Context;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DogLiveBot.Data.Repository.RepositoryImplementations;

public class ApplicationRepository<T> : IRepository<T>, IAsyncDisposable where T : BaseEntity<Guid>
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ApplicationRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<T?> GetFirstOrDefault(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.FirstOrDefaultAsync(filter, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<TResult?> GetFirstOrDefaultSelected<TResult>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Where(filter).Select(selector).FirstOrDefaultAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IfExists(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.AnyAsync(cancellationToken);
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
    public async Task AddRange(T[] entitys, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            foreach (var entity in entitys)
            {
                entity.CreateDate = DateTime.UtcNow;
            }

            await context.Set<T>().AddRangeAsync(entitys, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Update(T entity, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            entity.ModifiedDate = DateTime.UtcNow;

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
            var entity = await context.Set<T>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            if (entity is null)
            {
                return false;
            }

            entity.DeleteDate = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> Delete(T entity, CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var findAsync = await context.Set<T>().FirstOrDefaultAsync(s => s.Id == entity.Id, cancellationToken);
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
    public async Task<ICollection<T>> Where(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Where(filter).ToArrayAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<ICollection<TResult>> WhereSelected<TResult>(
        Expression<Func<T, bool>> filter, 
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Where(filter).Select(selector).ToArrayAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetLast(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken,
        bool getDeleted = false, 
        bool asNoTracking = true)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.OrderByDescending(s => s.CreateDate).LastOrDefaultAsync(filter, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task BatchUpdate(
        Expression<Func<T, bool>> filter,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateAction,
        CancellationToken cancellationToken)
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.Set<T>()
                .Where(filter)
                .ExecuteUpdateAsync(updateAction, cancellationToken: cancellationToken);
            
            await context.SaveChangesAsync(cancellationToken);
        }
    }


    private IQueryable<T> ApplyDeleteFilter(IQueryable<T> query, bool getDeleted)
    {
        if (!getDeleted)
        {
            query = query.Where(s => s.DeleteDate == null);
        }

        return query;
    }

    private IQueryable<T> ApplyAsNoTrackingFilter(IQueryable<T> query, bool asNoTracking)
    {
        return asNoTracking ? query.AsNoTracking() : query;
    }

    private IQueryable<T> PrepareQuery(ApplicationDbContext context, Expression<Func<T, bool>> filter, bool getDeleted, bool asNoTracking)
    {
        var query = context.Set<T>().Where(filter);

        query = ApplyDeleteFilter(query, getDeleted);
        query = ApplyAsNoTrackingFilter(query, asNoTracking);

        return query;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}