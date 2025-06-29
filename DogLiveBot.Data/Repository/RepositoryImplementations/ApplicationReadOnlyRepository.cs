using System.Linq.Expressions;
using DogLiveBot.Data.Context;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DogLiveBot.Data.Repository.RepositoryImplementations;

public class ApplicationReadOnlyRepository : IReadOnlyRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public ApplicationReadOnlyRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetFirstOrDefault<TEntity>(
        Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken, 
        bool getDeleted = false, 
        bool asNoTracking = true) 
        where TEntity : BaseEntity<Guid>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.FirstOrDefaultAsync(filter, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<TResult?> GetFirstOrDefaultSelected<TEntity, TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true) 
        where TEntity : BaseEntity<Guid>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Where(filter).Select(selector).FirstOrDefaultAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IfExists<TEntity>(
        Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true) 
        where TEntity : BaseEntity<Guid>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.AnyAsync(cancellationToken);
        }
    }

    

    /// <inheritdoc/>
    public async Task<ICollection<TEntity>> Where<TEntity>(
        Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true) 
        where TEntity : BaseEntity<Guid>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Where(filter).ToArrayAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<ICollection<TResult>> WhereSelected<TEntity, TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true) 
        where TEntity : BaseEntity<Guid>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Where(filter).Select(selector).ToArrayAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetLast<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken,
        bool getDeleted = false, 
        bool asNoTracking = true)
        where TEntity : BaseEntity<Guid>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.OrderByDescending(s => s.CreateDate).LastOrDefaultAsync(filter, cancellationToken);
        }
    }



    private IQueryable<TEntity> ApplyDeleteFilter<TEntity>(IQueryable<TEntity> query, bool getDeleted)
        where TEntity : BaseEntity<Guid>
    {
        if (!getDeleted)
        {
            query = query.Where(s => s.DeleteDate == null);
        }

        return query;
    }

    private IQueryable<TEntity> ApplyAsNoTrackingFilter<TEntity>(IQueryable<TEntity> query, bool asNoTracking)
        where TEntity : BaseEntity<Guid>
    {
        return asNoTracking ? query.AsNoTracking() : query;
    }

    private IQueryable<TEntity> PrepareQuery<TEntity>(ApplicationDbContext context, Expression<Func<TEntity, bool>> filter, bool getDeleted, bool asNoTracking)
        where TEntity : BaseEntity<Guid>
    {
        var query = context.Set<TEntity>().Where(filter);

        query = ApplyDeleteFilter(query, getDeleted);
        query = ApplyAsNoTrackingFilter(query, asNoTracking);

        return query;
    }
}