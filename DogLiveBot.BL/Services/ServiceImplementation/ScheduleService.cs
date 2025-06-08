using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Models.Options;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.Extensions.Options;

namespace DogLiveBot.BL.Services.ServiceImplementation;

/// <summary>
/// Сервис управления расписанием
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<AvailableSlot> _availableSlotRepository;
    private readonly IOptions<ApplicationOptions> _options;

    public ScheduleService(
        IRepository<Schedule> scheduleRepository,
        IRepository<AvailableSlot> availableSlotRepository,
        IOptions<ApplicationOptions> options)
    {
        _scheduleRepository = scheduleRepository;
        _availableSlotRepository = availableSlotRepository;
        _options = options;
    }

    /// <summary>
    /// Заполняет календарь временными интервалами на текущую неделю.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task FillCalendar(CancellationToken cancellationToken)
    {
        if (await HasActiveScheduleAsync(cancellationToken))
        {
            return;
        }

        await CreateNewScheduleAsync(cancellationToken);
    }

    /// <summary>
    /// Проверяет наличие активного расписания на текущую неделю.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если активное расписание существует, иначе false.</returns>
    private async Task<bool> HasActiveScheduleAsync(CancellationToken cancellationToken)
    {
        return await _scheduleRepository.IfExists(
            filter: s => DateTime.UtcNow.Date > s.WeekStartDate &&
                         DateTime.UtcNow.Date < s.WeekEndDate &&
                         s.IsActiveWeek,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Создает новое расписание и генерирует доступные временные слоты.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    private async Task CreateNewScheduleAsync(CancellationToken cancellationToken)
    {
        // Создаем новое расписание
        var schedule = new Schedule
        {
            WeekStartDate = DateTime.UtcNow,
            WeekEndDate = DateTime.UtcNow.AddDays(7),
            IsActiveWeek = true
        };

        await _scheduleRepository.Add(schedule, cancellationToken);

        var settings = _options.Value.AvailableSlotSettings;
        var availableSlots = 
            Enum.GetValues<DayOfWeek>()
                .SelectMany(dayOfWeek => 
                    GenerateAvailableSlots(schedule.Id, dayOfWeek, settings.GetStartTimeSpan, settings.GetEndTimeSpan, settings.GetIntervalSpan))
                .ToArray();

        // Сохраняем интервалы
        await _availableSlotRepository.AddRange(availableSlots, cancellationToken);
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
        Guid scheduleId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, TimeSpan interval)
    {
        for (var time = startTime; time < endTime; time = time.Add(interval))
        {
            var result = new AvailableSlot
            {
                ScheduleId = scheduleId,
                DayOfWeek = dayOfWeek,
                Date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day + (int)dayOfWeek).ToUniversalTime(),
                StartTime = time,
                EndTime = time.Add(interval),
                IsAvailable = true
            };;

            yield return result;
        }
    }
}