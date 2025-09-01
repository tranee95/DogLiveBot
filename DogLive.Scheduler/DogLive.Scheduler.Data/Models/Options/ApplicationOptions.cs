namespace DogLive.Scheduler.Data.Models.Options;

/// <summary>
/// Настройки приложения.
/// </summary>
public class ApplicationOptions
{
    /// <summary>
    /// Конфигурация крон выражений.
    /// </summary>
    public CronExpressionSettings CronExpressionSettings { get; set; }

    /// <summary>
    /// Настройки формирования слотов расписания
    /// </summary>
    public ScheduleSlotSettings ScheduleSlotSettings { get; set; }

    /// <summary>
    /// Настройка подключения к RabbitMQ
    /// </summary>
    public RabbitMqSettings RabbitMqSettings { get; set; }
}