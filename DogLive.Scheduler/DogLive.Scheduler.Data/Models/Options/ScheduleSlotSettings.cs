namespace DogLive.Scheduler.Data.Models.Options;

/// <summary>
/// Настройки формирования слотов расписания
/// </summary>
public class ScheduleSlotSettings
{
    /// <summary>
    /// Интервал (час)
    /// </summary>
    public int Interval { get; set; }

    /// <summary>
    /// Время начала (час).
    /// </summary>
    public int StartTime { get; set; }

    /// <summary>
    /// Время окончания (час).
    /// </summary>
    public int EndTime { get; set; }

    /// <summary>
    /// Возвращает время начала в формате <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan GetStartTimeSpan => TimeSpan.FromHours(StartTime);

    /// <summary>
    /// Возвращает время окончания в формате <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan GetEndTimeSpan => TimeSpan.FromHours(EndTime);

    /// <summary>
    /// Возвращает интервал <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan GetIntervalSpan => TimeSpan.FromHours(Interval);
}