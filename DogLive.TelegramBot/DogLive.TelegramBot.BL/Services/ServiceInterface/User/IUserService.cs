namespace DogLive.TelegramBot.BL.Services.ServiceInterface.User;

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
    Task<bool> CreateIfNotExistAsync(Data.Context.Entity.User user, CancellationToken cancellationToken);
}