namespace DogLiveBot.Core.Options;

/// <summary>
/// Настройки приложения.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// Настройки для Telegram-бота.
    /// </summary>
    public TelegramBotSettings TelegramBotSettings { get; set; }

    /// <summary>
    /// Настройки подключения к базе данных приложения.
    /// </summary>
    public ApplicationDbConnection ApplicationDbConnection { get; set; }
}