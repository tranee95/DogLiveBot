namespace DogLiveBot.Data.Entity;

/// <summary>
/// Представляет пользователя в системе.
/// </summary>
public class User : BaseEntity<Guid>
{
    /// <summary>
    /// Идентификатор пользователя в Telegram.
    /// </summary>
    public long TelegramId { get; set; }

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

    /// <summary>
    /// Собаки пользователя
    /// </summary>
    public ICollection<Dog> Dogs { get; set; } = new List<Dog>();

    /// <summary>
    /// сущность, содержащую информацию о
    /// запросах вызова пользователей в приложении.
    /// </summary>
    public UserCallbackQuery UserCallbackQuery { get; set; }

}