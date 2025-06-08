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
    private readonly ApplicationDbContext _context;
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IRepository<AvailableSlot> _availableSlotRepository;
    private readonly IOptions<ApplicationOptions> _options;

    public ScheduleService(
        ILogger<TelegramBotService> logger,
        IRepository<Schedule> scheduleRepository,
        IRepository<AvailableSlot> availableSlotRepository,
        IOptions<ApplicationOptions> options, 
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
        return await _scheduleRepository.IfExists(
            filter: s => DateTime.Now.ToUniversalTime() >= s.WeekStartDate.ToUniversalTime() &&
                         DateTime.Now.ToUniversalTime() <= s.WeekEndDate.ToUniversalTime() && 
                         s.IsActiveWeek == true,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Создает новое расписание и генерирует доступные временные слоты.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    private async Task CreateNewSchedule(CancellationToken cancellationToken)
    {
        await using (var tr = await _context.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                await DeactivatingActiveSchedules(cancellationToken);
                var schedule = new Schedule(GetStartOfWeek(DateTime.Now), GetEndOfWeek(DateTime.Now), true);

                await _scheduleRepository.Add(schedule, cancellationToken);
                await _availableSlotRepository.AddRange(CreateAvailableSlot(schedule.Id), cancellationToken);
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
        await _scheduleRepository.BatchUpdate(
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
        return date.AddDays(-diff).Date.ToUniversalTime();
    }

    /// <summary>
    /// Получает дату конца недели (воскресенье) для указанной даты.
    /// </summary>
    /// <param name="date">Дата, для которой нужно найти конец недели.</param>
    /// <returns>Дата конца недели (воскресенье).</returns>
    private DateTime GetEndOfWeek(DateTime date)
    {
        return GetStartOfWeek(date).AddDays(6).ToUniversalTime();
    }
}