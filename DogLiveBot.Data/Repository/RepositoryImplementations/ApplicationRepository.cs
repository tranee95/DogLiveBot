using System.Linq.Expressions;
using DogLiveBot.Data.Context;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace DogLiveBot.Data.Repository.RepositoryImplementations;

public class ApplicationRepository : IRepository, IAsyncDisposable
{ 
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    
    private ApplicationDbContext? _transactionContext;

    public ApplicationRepository(
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
        _transactionContext = null;
    }

    /// <inheritdoc/>
    public async Task<IDbContextTransaction> CreateTransaction(CancellationToken cancellationToken)
    {
        _transactionContext = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _transactionContext.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task Add<TEntity>(TEntity entity, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await AddEntity(context, entity, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Add<TEntity>(TEntity entity, IDbContextTransaction transaction, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        if (_transactionContext is null)
        {
            throw new NullReferenceException("Transaction context is null");
        }

        await _transactionContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        await AddEntity<TEntity>(_transactionContext, entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task AddRange<TEntity>(TEntity[] entitys, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await AddRangeEntity<TEntity>(context, entitys, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task AddRange<TEntity>(TEntity[] entitys, IDbContextTransaction transaction, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        if (_transactionContext is null)
        {
            throw new NullReferenceException("Transaction context is null");
        }

        await _transactionContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        await AddRangeEntity<TEntity>(_transactionContext, entitys, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task Update<TEntity>(TEntity entity, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await UpdateEntity<TEntity>(context, entity, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Update<TEntity>(TEntity entity, IDbContextTransaction transaction, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        if (_transactionContext is null)
        {
            throw new NullReferenceException("Transaction context is null");
        }

        await _transactionContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        await UpdateEntity<TEntity>(_transactionContext, entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> Delete<TEntity>(int id, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await DeleteEntity<TEntity>(context, id, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> Delete<TEntity>(int id, IDbContextTransaction transaction,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        if (_transactionContext is null)
        {
            throw new NullReferenceException("Transaction context is null");
        }

        await _transactionContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        return await DeleteEntity<TEntity>(_transactionContext, id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> Delete<TEntity>(TEntity entity, CancellationToken cancellationToken) 
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            return await DeleteEntity<TEntity>(context, entity, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> Delete<TEntity>(TEntity entity, IDbContextTransaction transaction,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        if (_transactionContext is null)
        {
            throw new NullReferenceException("Transaction context is null");
        }
        
        await _transactionContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        return await DeleteEntity<TEntity>(_transactionContext, entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task BatchUpdate<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateAction,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        await using (var context = await _contextFactory.CreateDbContextAsync(cancellationToken))
        {
            await context.Set<TEntity>()
                .Where(filter)
                .ExecuteUpdateAsync(updateAction, cancellationToken: cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_transactionContext is not null)
        {
            await _transactionContext.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_transactionContext is not null)
        {
            _transactionContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
    
    private async Task AddEntity<TEntity>(ApplicationDbContext context, TEntity entity, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        entity.CreateDate = DateTime.UtcNow;

        await context.Set<TEntity>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task AddRangeEntity<TEntity>(ApplicationDbContext context, TEntity[] entitys, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        foreach (var entity in entitys)
        {
            entity.CreateDate = DateTime.UtcNow;
        }

        await context.Set<TEntity>().AddRangeAsync(entitys, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateEntity<TEntity>(ApplicationDbContext context, TEntity entity, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        entity.ModifiedDate = DateTime.UtcNow;
        var existing = await context.Set<TEntity>()
            .FirstOrDefaultAsync(e => e.Id == entity.Id, cancellationToken);

        if (existing is null)
        {
            throw new InvalidOperationException($"{typeof(TEntity).Name} with Id={entity.Id} does not exist and cannot be updated.");
        }

        context.Entry(existing).CurrentValues.SetValues(entity);
        existing.ModifiedDate = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> DeleteEntity<TEntity>(ApplicationDbContext context, int id, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        var entity = await context.Set<TEntity>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.DeleteDate = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
    
    private async Task<bool> DeleteEntity<TEntity>(ApplicationDbContext context, TEntity entity, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>
    {
        var deleteEntity = await context.Set<TEntity>().FirstOrDefaultAsync(s => s.Id == entity.Id, cancellationToken);
        if (deleteEntity is null)
        {
            return false;
        }

        deleteEntity.DeleteDate = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}