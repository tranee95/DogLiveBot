namespace DogLiveBot.Core.Options;

/// <summary>
/// Настройки подключения к базе данных приложения.
/// </summary>
public class ApplicationDbConnection
{
    /// <summary>
    /// Строка подключения к базе данных.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Таймаут подключения к базе данных в секундах.
    /// </summary>
    public int Timeout { get; set; }
}