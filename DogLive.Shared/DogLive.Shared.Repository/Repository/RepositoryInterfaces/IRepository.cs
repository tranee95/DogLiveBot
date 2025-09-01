using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Query;
using Shared.Messages.Repository.Repository.Entitys;

namespace Shared.Messages.Repository.Repository.RepositoryInterfaces;

public interface IRepository : IDisposable
{
    /// <summary>
    /// Создает новую транзакцию.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Объект транзакции базы данных.</returns>
    Task<IDbContextTransaction> CreateTransaction(CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет новую сущность.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entity">Добавляемая сущность.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task Add<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Добавляет новую сущность с использованием транзакции.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entity">Добавляемая сущность.</param>
    /// <param name="transaction">Идентификатор транзакции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task Add<TEntity>(TEntity entity, IDbContextTransaction transaction, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Добавляет массив сущностей.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entities">Список добавляемых сущностей.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task AddRange<TEntity>(TEntity[] entities, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Добавляет массив сущностей с использованием транзакции.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entities">Список добавляемых сущностей.</param>
    /// <param name="transaction">Идентификатор транзакции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task AddRange<TEntity>(TEntity[] entities, IDbContextTransaction transaction, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Обновляет сущность.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entity">Обновляемая сущность.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task Update<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Обновляет сущность с использованием транзакции.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entity">Обновляемая сущность.</param>
    /// <param name="transaction">Идентификатор транзакции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task Update<TEntity>(TEntity entity, IDbContextTransaction transaction, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Удаляет сущность по идентификатору.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если успешно удалено; иначе — false.</returns>
    Task<bool> Delete<TEntity>(int id, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Удаляет сущность по идентификатору с использованием транзакции.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="transaction">Идентификатор транзакции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если успешно удалено; иначе — false.</returns>
    Task<bool> Delete<TEntity>(int id, IDbContextTransaction transaction, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Удаляет указанную сущность.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entity">Удаляемая сущность.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если успешно удалено; иначе — false.</returns>
    Task<bool> Delete<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Удаляет указанную сущность с учетом транзакции.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entity">Удаляемая сущность.</param>
    /// <param name="transaction">Идентификатор транзакции.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если успешно удалено; иначе — false.</returns>
    Task<bool> Delete<TEntity>(TEntity entity, IDbContextTransaction transaction, CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;
    
    /// <summary>
    /// Выполняет пакетное обновление сущностей на основе фильтра.
    /// </summary>
    Task BatchUpdate<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateAction,
        CancellationToken cancellationToken)
        where TEntity : BaseEntity<int>;
}