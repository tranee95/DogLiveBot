using DogLiveBot.Data.Entity;

namespace DogLiveBot.BL.Services.ServiceInterface;

/// <summary>
/// Интерфейс для управления пользователями.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Создает пользователя, если он не существует.
    /// </summary>
    /// <param name="user">Пользователь, который должен быть создан.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<bool> CreateIfNotExistAsync(User user, CancellationToken cancellationToken);
}