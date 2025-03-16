using System.Linq.Expressions;
using DogLiveBot.Data.Entity;

namespace DogLiveBot.Data.Repository.RepositoryInterfaces;

public interface IRepository<T> : IDisposable where T : BaseEntity<Guid>
{
    /// <summary>
    /// Получает все сущности из репозитория.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Список всех сущностей.</returns>
    Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Получает сущность.
    /// </summary>
    /// <param name="func">Функция фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <returns>Сущность или null, если не найдена.</returns>
    Task<T?> Get(Expression<Func<T, bool>> func, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получает сущность по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Сущность или null, если не найдена.</returns>
    Task<T?> GetById(Guid id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получает наличие сущности.
    /// </summary>
    /// <param name="func">Функция фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Наличие сущности</returns>
    Task<bool> IfExists(Func<T, bool> func, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет новую сущность в репозиторий.
    /// </summary>
    /// <param name="entity">Сущность для добавления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task Add(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет существующую сущность в репозитории.
    /// </summary>
    /// <param name="entity">Сущность для обновления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task Update(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет сущность по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> Delete(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет указанную сущность.
    /// </summary>
    /// <param name="entity">Сущность для удаления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> Delete(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Выполняет жесткое удаление сущности по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> HardDelete(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Выполняет жесткое удаление указанной сущности.
    /// </summary>
    /// <param name="entity">Сущность для жесткого удаления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> HardDelete(T? entity, CancellationToken cancellationToken);

    /// <summary>
    /// Фильтрует сущности по заданному условию.
    /// </summary>
    /// <param name="func">Функция фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Коллекция сущностей, соответствующих условию.</returns>
    Task<ICollection<T>> Where(Func<T, bool> func, CancellationToken cancellationToken);

    /// <summary>
    /// Получает последнюю сущность.
    /// </summary>
    /// <param name="func">Функция фильтрации.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> GetLast(Expression<Func<T, bool>> func, CancellationToken cancellationToken);

    /// <summary>
    /// Сохраняет изменения в репозитории.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task Save(CancellationToken cancellationToken);
}