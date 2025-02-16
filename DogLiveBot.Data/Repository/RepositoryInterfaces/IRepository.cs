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
    /// Получает сущность по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <returns>Сущность или null, если не найдена.</returns>
    T? GetById(Guid id);

    /// <summary>
    /// Добавляет новую сущность в репозиторий.
    /// </summary>
    /// <param name="entity">Сущность для добавления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Обновляет существующую сущность в репозитории.
    /// </summary>
    /// <param name="entity">Сущность для обновления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Получает сущность по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Сущность или null, если не найдена.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет сущность по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет указанную сущность.
    /// </summary>
    /// <param name="entity">Сущность для удаления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет несколько сущностей.
    /// </summary>
    /// <param name="entities">Коллекция сущностей для удаления.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    bool Delete(ICollection<T> entities);

    /// <summary>
    /// Выполняет жесткое удаление сущности по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> HardDeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Выполняет жесткое удаление указанной сущности.
    /// </summary>
    /// <param name="entity">Сущность для жесткого удаления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    Task<bool> HardDeleteAsync(T? entity, CancellationToken cancellationToken);

    /// <summary>
    /// Жестко удаляет несколько сущностей.
    /// </summary>
    /// <param name="entities">Коллекция сущностей для жесткого удаления.</param>
    /// <returns>Истина, если удаление прошло успешно; иначе - ложь.</returns>
    bool HardDelete(ICollection<T>? entities);

    /// <summary>
    /// Фильтрует сущности по заданному условию.
    /// </summary>
    /// <param name="func">Функция фильтрации.</param>
    /// <returns>Коллекция сущностей, соответствующих условию.</returns>
    ICollection<T> Where(Func<T, bool> func);

    /// <summary>
    /// Сохраняет изменения в репозитории.
    /// </summary>
    void Save();

    /// <summary>
    /// Сохраняет изменения в репозитории.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task SaveAsync(CancellationToken cancellationToken);
}