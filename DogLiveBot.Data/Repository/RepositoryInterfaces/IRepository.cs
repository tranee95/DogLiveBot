using System.Linq.Expressions;
using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore.Query;

namespace DogLiveBot.Data.Repository.RepositoryInterfaces;

public interface IRepository<T> : IDisposable where T : BaseEntity<Guid>
{
    /// <summary>
    /// Получает сущность.
    /// </summary>
    /// <param name="filter">Функция фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <param name="getDeleted">Учитывать удаленные записи</param>
    /// <param name="asNoTracking">Применять кеширование запроса</param>
    /// <returns>Сущность или null, если не найдена.</returns>
    Task<T?> GetFirstOrDefault(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true);

    /// <summary>
    /// Получает выбранные данные из сущности, соответствующей заданному фильтру.
    /// </summary>
    /// <param name="filter">Функция фильтрации, определяющая условия для поиска сущности.</param>
    /// <param name="selector">Функция выбора, определяющая, какие данные должны быть возвращены.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <param name="getDeleted">Учитывать удаленные записи</param>
    /// <param name="asNoTracking">Применять кеширование запроса</param>
    /// <returns>Выбранные данные типа <typeparamref name="TResult"/> или <c>null</c>, если сущность не найдена.</returns>
    Task<TResult?> GetFirstOrDefaultSelected<TResult>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true);

    /// <summary>
    /// Получает наличие сущности.
    /// </summary>
    /// <param name="filter">Функция фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <param name="getDeleted">Учитывать удаленные записи</param>
    /// <param name="asNoTracking">Применять кеширование запроса</param>
    /// <returns>Наличие сущности</returns>
    Task<bool> IfExists(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true);

    /// <summary>
    /// Добавляет новую сущность в репозиторий.
    /// </summary>
    /// <param name="entity">Сущность для добавления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task Add(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет массив сущностей в репозиторий.
    /// </summary>
    /// <param name="entitys">Сущности для добавления.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    Task AddRange(T[] entitys, CancellationToken cancellationToken);

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
    /// Фильтрует сущности по заданному условию.
    /// </summary>
    /// <param name="filter">Функция фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <param name="getDeleted">Учитывать удаленные записи</param>
    /// <param name="asNoTracking">Применять кеширование запроса</param>
    /// <returns>Коллекция сущностей, соответствующих условию.</returns>
    Task<ICollection<T>> Where(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true);

    /// <summary>
    /// Получает выбранные данные из сущности, соответствующей заданному фильтру.
    /// </summary>
    /// <param name="filter">Функция фильтрации, определяющая условия для поиска сущности.</param>
    /// <param name="selector">Функция выбора, определяющая, какие данные должны быть возвращены.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <param name="getDeleted">Учитывать удаленные записи</param>
    /// <param name="asNoTracking">Применять кеширование запроса</param>
    /// <returns>Выбранные данные типа <typeparamref name="TResult"/> или <c>null</c>, если сущность не найдена.</returns>
    Task<ICollection<TResult>> WhereSelected<TResult>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true);

    /// <summary>
    /// Получает последнюю сущность.
    /// </summary>
    /// <param name="filter">Функция фильтрации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции</param>
    /// <param name="getDeleted">Учитывать удаленные записи</param>
    /// <param name="asNoTracking">Применять кеширование запроса</param>
    /// <returns>Выбранные данные типа <typeparamref name="T"/> или <c>null</c>, если сущность не найдена.</returns>
    Task<T?> GetLast(
        Expression<Func<T, bool>> filter, 
        CancellationToken cancellationToken,
        bool getDeleted = false,
        bool asNoTracking = true);

    /// <summary>
    /// Выполняет пакетное обновление сущностей в базе данных на основе указанного фильтра и действий обновления.
    /// </summary>
    /// <param name="filter">Фильтр, определяющий, какие сущности должны быть обновлены.</param>
    /// <param name="updateAction">Действия обновления, определяющие, какие свойства и как нужно изменить.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <typeparam name="T">Тип сущности, над которой выполняется обновление.</typeparam>
    Task BatchUpdate(
        Expression<Func<T, bool>> filter,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateAction,
        CancellationToken cancellationToken);
}