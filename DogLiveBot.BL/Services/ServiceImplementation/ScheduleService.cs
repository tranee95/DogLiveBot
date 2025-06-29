using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Context;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Models.Options;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DogLiveBot.BL.Services.ServiceImplementation;

/// <summary>
/// Сервис управления расписанием
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly ILogger<TelegramBotService> _logger;
    private readonly IOptions<ApplicationOptions> _options;
    private readonly IChangeRepository _changeRepository;
    private readonly IReadOnlyRepository _readOnlyRepository;

    public ScheduleService(
        ILogger<TelegramBotService> logger,
        IOptions<ApplicationOptions> options, 
        IChangeRepository changeRepository, 
        IReadOnlyRepository readOnlyRepository)
    {
        _logger = logger;
        _options = options;
        _changeRepository = changeRepository;
        _readOnlyRepository = readOnlyRepository;
    }

    /// <summary>
    /// Заполняет календарь временными интервалами на текущую неделю.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task FillCalendar(CancellationToken cancellationToken)
    {
        if (await HasActiveSchedule(cancellationToken))
        {
            return;
        }

        await CreateNewSchedule(cancellationToken);
    }

    /// <summary>
    /// Проверяет наличие активного расписания на текущую неделю.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если активное расписание существует, иначе false.</returns>
    private async Task<bool> HasActiveSchedule(CancellationToken cancellationToken)
    {
        var now = DateTime.Now;

        return await _readOnlyRepository.IfExists<Schedule>(
            filter: s => now >= s.WeekStartDate && now <= s.WeekEndDate && s.IsActiveWeek,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Создает новое расписание и генерирует доступные временные слоты.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    private async Task CreateNewSchedule(CancellationToken cancellationToken)
    {
        await using (var tr = await _changeRepository.CreateTransaction(cancellationToken))
        {
            try
            {
                await DeactivatingActiveSchedules(cancellationToken);
                
                var schedule = new Schedule(GetStartOfWeek(DateTime.Today), GetEndOfWeek(DateTime.Today), true);
                await _changeRepository.Add<Schedule>(schedule, tr, cancellationToken);

                var availableSlot = CreateAvailableSlot(schedule.Id);
                await _changeRepository.AddRange<AvailableSlot>(availableSlot, tr, cancellationToken);

                await tr.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error to create NewSchedule: {ex.Message}");
                await tr.RollbackAsync(cancellationToken);
            }
        }
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
            yield return new AvailableSlot(scheduleId, dayOfWeek, time, interval);;
        }
    }

    /// <summary>
    /// Создает массив доступных слотов на основе идентификатора расписания и настроек доступных слотов.
    /// </summary>
    /// <param name="scheduleId">Идентификатор расписания, для которого создаются слоты.</param>
    /// <returns>Массив доступных слотов.</returns>
    private AvailableSlot[] CreateAvailableSlot(Guid scheduleId)
    {
        var settings = _options.Value.AvailableSlotSettings;
        return Enum.GetValues<DayOfWeek>()
            .SelectMany(dayOfWeek =>
                GenerateAvailableSlots(scheduleId, dayOfWeek, settings.GetStartTimeSpan,
                    settings.GetEndTimeSpan, settings.GetIntervalSpan))
            .ToArray();
    }

    /// <summary>
    /// Отключает активные расписания, устанавливая их свойство IsActiveWeek в false.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    private async Task DeactivatingActiveSchedules(CancellationToken cancellationToken)
    {
        await _changeRepository.BatchUpdate<Schedule>(
            filter: s => s.IsActiveWeek,
            updateAction: props => props.SetProperty(s => s.IsActiveWeek, false),
            cancellationToken: cancellationToken);
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