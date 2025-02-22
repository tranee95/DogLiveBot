namespace DogLiveBot.Data.Entity;

/// <summary>
/// Представляет пользователя в системе.
/// </summary>
public class User : BaseEntity<Guid>
{
    /// <summary>
    /// Идентификатор пользователя в Telegram.
    /// </summary>
    /// <value>
    /// Идентификатор пользователя в Telegram. Может быть <c>null</c>, если пользователь не зарегистрирован в Telegram.
    /// </value>
    public long? TelegramId { get; set; }

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    /// <value>
    /// Номер телефона пользователя. Не может быть <c>null</c>, должен быть инициализирован.
    /// </value>
    public string PhoneNumber { get; set; } = default!;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public string? LastName { get; set; }
}