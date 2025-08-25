namespace DogLiveBot.BL.Services.ServiceInterface.Cache;

public interface ICacheService
{
    /// <summary>
    /// Сохраняет данные в кеш по заданному ключу.
    /// </summary>
    /// <param name="key">Ключ для хранения данных.</param>
    /// <param name="data">Сохраняемые данные.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task Set(string key, string data, CancellationToken cancellationToken);
    
    /// <summary>
    /// Извлекает данные из кеша по заданному ключу.
    /// </summary>
    /// <param name="key">Ключ для поиска данных.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные, сохранённые по ключу, или null, если данные не найдены.</returns>
    public Task<string> Get(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет данные из кеша по заданному ключу.
    /// </summary>
    /// <param name="key">Ключ для удаления данных.</param>
    /// <param name="cancellationToke">Токен отмены операции.</param>
    public Task Remove(string key, CancellationToken cancellationToke);
}