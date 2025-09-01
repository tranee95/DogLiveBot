using DogLive.Scheduler.BL.Services.ServiceInterface.Schedule;
using DogLive.Scheduler.Data.Models;
using DogLive.Scheduler.Data.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Messages.Messages.Schedule.Model;
using Shared.Messages.Messages.Service.ServiceInterface;

namespace DogLive.Scheduler.BL.Services.ServiceImplementation.Schedule;

/// <summary>
/// Сервис управления расписанием
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly IOptions<ApplicationOptions> _options;
    private readonly IMassTransitService _massTransitService;

    public ScheduleService(
        ILogger<ScheduleService> logger,
        IOptions<ApplicationOptions> options, 
        IMassTransitService massTransitService)
    {
        _logger = logger;
        _options = options;
        _massTransitService = massTransitService;
    }

    /// <inheritdoc/>
    public async Task CreateAndSendAvalableSlot(CancellationToken cancellationToken)
    {
        var rmq = new RMQScheduleSlot(CreateScheduleSlot());
        await _massTransitService.Publish<IRMQScheduleSlot>(rmq);
    }

    /// <summary>
    /// Создает массив доступных слотов на основе идентификатора расписания и настроек доступных слотов.
    /// </summary>
    /// <returns>Массив доступных слотов.</returns>
    private ScheduleSlot[] CreateScheduleSlot()
    {
        var settings = _options.Value.ScheduleSlotSettings;
        return Enum.GetValues<DayOfWeek>()
            .SelectMany(dayOfWeek =>
                GenerateAvailableSlots(dayOfWeek, settings.GetStartTimeSpan,
                    settings.GetEndTimeSpan, settings.GetIntervalSpan))
            .ToArray();
    }

    /// <summary>
    /// Генерирует доступные временные интервалы для указанного дня недели.
    /// </summary>
    /// <param name="dayOfWeek">День недели.</param>
    /// <param name="startTime">Начальное время.</param>
    /// <param name="endTime">Конечное время.</param>
    /// <param name="interval">Интервал между слотами.</param>
    /// <returns>Список доступных временных интервалов.</returns>
    private IEnumerable<ScheduleSlot> GenerateAvailableSlots(
        DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, TimeSpan interval)
    {
        for (var time = startTime; time < endTime; time = time.Add(interval))
        {
            yield return new ScheduleSlot(dayOfWeek, time, interval);;
        }
    }
}