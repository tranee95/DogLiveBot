using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Messages.Repository.Repository.Context;
using Shared.Messages.Repository.Repository.Entitys;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;

namespace Shared.Messages.Repository.Repository.RepositoryImplementations;

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
        where TEntity : BaseEntity<int>
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
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IfExists<TEntity>(
        Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.AnyAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<ICollection<TEntity>> Get<TEntity>(
        Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.ToArrayAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<ICollection<TResult>> GetSelected<TEntity, TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.Select(selector).ToArrayAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetLast<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken,
        bool getDeleted = false, 
        bool asNoTracking = true)
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            var query = PrepareQuery(context, filter, getDeleted, asNoTracking);
            return await query.OrderByDescending(s => s.CreateDate).FirstOrDefaultAsync(cancellationToken);
        }
    }



    private IQueryable<TEntity> ApplyDeleteFilter<TEntity>(IQueryable<TEntity> query, bool getDeleted)
        where TEntity : BaseEntity<int>
    {
        if (!getDeleted)
        {
            query = query.Where(s => s.DeleteDate == null);
        }

        return query;
    }

    private IQueryable<TEntity> ApplyAsNoTrackingFilter<TEntity>(IQueryable<TEntity> query, bool asNoTracking)
        where TEntity : BaseEntity<int>
    {
        return asNoTracking ? query.AsNoTracking() : query;
    }

    private IQueryable<TEntity> PrepareQuery<TEntity>(ApplicationDbContext context, Expression<Func<TEntity, bool>> filter, bool getDeleted, bool asNoTracking)
        where TEntity : BaseEntity<int>
    {
        var query = context.Set<TEntity>().Where(filter);

        query = ApplyDeleteFilter(query, getDeleted);
        query = ApplyAsNoTrackingFilter(query, asNoTracking);

        return query;
    }
}