using System.Linq.Expressions;
using DogLiveBot.Data.Context.Entity;

namespace DogLiveBot.Data.Repository.RepositoryInterfaces;

public interface IReadOnlyRepository
{
    /// <summary>
    /// Получает первую сущность, соответствующую заданному фильтру.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="filter">Условия фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <param name="getDeleted">Учитывать ли записи, помеченные как удаленные.</param>
    /// <param name="asNoTracking">Использовать ли режим без отслеживания.</param>
    /// <returns>Первая найденная сущность или null, если таких нет.</returns>
    Task<TEntity?> GetFirstOrDefault<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Получает первую сущность по указанному фильтру и проецирует результат.
    /// </summary>
    /// <typeparam name="TResult">Тип результата проекции.</typeparam>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="filter">Условия фильтрации.</param>
    /// <param name="selector">Проекция результата.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="getDeleted">Учитывать ли удаленные записи.</param>
    /// <param name="asNoTracking">Использовать ли режим без отслеживания.</param>
    /// <returns>Результат проекции или null.</returns>
    Task<TResult?> GetFirstOrDefaultSelected<TEntity, TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Проверяет наличие сущности, удовлетворяющей заданным условиям.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="filter">Условия фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <param name="getDeleted">Учитывать ли записи, помеченные как удаленные.</param>
    /// <param name="asNoTracking">Использовать ли режим без отслеживания.</param>
    /// <returns>True, если сущности существуют; иначе — false.</returns>
    Task<bool> IfExists<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Возвращает коллекцию сущностей, соответствующих указанному условию.
    /// </summary>
    Task<ICollection<TEntity>> Where<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Возвращает коллекцию проекций на основе сущностей по фильтру.
    /// </summary>
    Task<ICollection<TResult>> GetSelected<TEntity, TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
        where TEntity : BaseEntity<int>;

    /// <summary>
    /// Получает последнюю сущность из выборки.
    /// </summary>
    Task<TEntity?> GetLast<TEntity>(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true)
        where TEntity : BaseEntity<int>;
}