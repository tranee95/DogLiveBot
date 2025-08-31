using DogLive.Scheduler.BL.Services.ServiceInterface.Schedule;
using DogLive.Scheduler.Data.Models;
using DogLive.Scheduler.Data.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DogLive.Scheduler.BL.Services.ServiceImplementation.Schedule;

/// <summary>
/// Сервис управления расписанием
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly IOptions<ApplicationOptions> _options;

    public ScheduleService(
        ILogger<ScheduleService> logger,
        IOptions<ApplicationOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task FillCalendar(CancellationToken cancellationToken)
    {
        await CreateNewSchedule(cancellationToken);
    }

    /// <summary>
    /// Создает новое расписание и генерирует доступные временные слоты.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    private async Task CreateNewSchedule(CancellationToken cancellationToken)
    {
        var availableSlot = CreateAvailableSlot();
    }

    /// <summary>
    /// Создает массив доступных слотов на основе идентификатора расписания и настроек доступных слотов.
    /// </summary>
    /// <param name="scheduleId">Идентификатор расписания, для которого создаются слоты.</param>
    /// <returns>Массив доступных слотов.</returns>
    private AvailableSlot[] CreateAvailableSlot()
    {
        var settings = _options.Value.AvailableSlotSettings;
        return Enum.GetValues<DayOfWeek>()
            .SelectMany(dayOfWeek =>
                GenerateAvailableSlots(dayOfWeek, settings.GetStartTimeSpan,
                    settings.GetEndTimeSpan, settings.GetIntervalSpan))
            .ToArray();
    }

    /// <summary>
    /// Генерирует доступные временные интервалы для указанного дня недели.
    /// </summary>
    /// <param name="scheduleId">Идентификатор расписания.</param>
    /// <param name="dayOfWeek">День недели.</param>
    /// <param name="startTime">Начальное время.</param>
    /// <param name="endTime">Конечное время.</param>
    /// <param name="interval">Интервал между слотами.</param>
    /// <returns>Список доступных временных интервалов.</returns>
    private IEnumerable<AvailableSlot> GenerateAvailableSlots(
        DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, TimeSpan interval)
    {
        for (var time = startTime; time < endTime; time = time.Add(interval))
        {
            yield return new AvailableSlot(dayOfWeek, time, interval);;
        }
    }

    /// <summary>
    /// Получает дату начала недели (понедельник) для указанной даты.
    /// </summary>
    /// <param name="date">Дата, для которой нужно найти начало недели.</param>
    /// <returns>Дата начала недели (понедельник).</returns>
    private DateTime GetStartOfWeek(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    /// <summary>
    /// Получает дату конца недели (воскресенье) для указанной даты.
    /// </summary>
    /// <param name="date">Дата, для которой нужно найти конец недели.</param>
    /// <returns>Дата конца недели (воскресенье).</returns>
    private DateTime GetEndOfWeek(DateTime date)
    {
        return GetStartOfWeek(date).AddDays(6).AddDays(1).AddSeconds(-1);
    }
}