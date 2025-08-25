using System.Globalization;
using DogLiveBot.BL.Services.ServiceImplementation.Telegram;
using DogLiveBot.BL.Services.ServiceInterface.Schedule;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Models.Options;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DogLiveBot.BL.Services.ServiceImplementation.Schedule;

/// <summary>
/// Сервис управления расписанием
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly ILogger<TelegramBotService> _logger;
    private readonly IOptions<ApplicationOptions> _options;
    private readonly IRepository _repository;
    private readonly IReadOnlyRepository _readOnlyRepository;

    public ScheduleService(
        ILogger<TelegramBotService> logger,
        IOptions<ApplicationOptions> options, 
        IRepository repository, 
        IReadOnlyRepository readOnlyRepository)
    {
        _logger = logger;
        _options = options;
        _repository = repository;
        _readOnlyRepository = readOnlyRepository;
    }

    /// <inheritdoc/>
    public async Task FillCalendar(CancellationToken cancellationToken)
    {
        if (await HasActiveSchedule(cancellationToken))
        {
            return;
        }

        await CreateNewSchedule(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ICollection<DaysDto>> GetActiveDays(CancellationToken cancellationToken)
    {
        var activeSchedule = await _readOnlyRepository.GetFirstOrDefault<Data.Context.Entity.Schedule>(
            filter: s => s.IsActiveWeek,
            cancellationToken: cancellationToken);
        
        var days = new List<DaysDto>();
        for (var date = activeSchedule.WeekStartDate; date <= activeSchedule.WeekEndDate; date = date.AddDays(1))
        {
            if (date >= DateTime.UtcNow)
            {
                days.Add(new DaysDto() { 
                    Text = $"{date.ToShortDateString()} ({date.ToString("dddd", new CultureInfo("ru-RU"))})",
                    DayOfWeek = date.DayOfWeek
                });
            }
        }

        return days;
    }

    /// <inheritdoc/>
    public async Task<ICollection<AvailableTimeDto>> GetAvailableTimes(DayOfWeek dayOfWeek,
        CancellationToken cancellationToken)
    {
        var activeSchedule = await _readOnlyRepository.GetFirstOrDefault<Data.Context.Entity.Schedule>(
            filter: s => s.IsActiveWeek,
            cancellationToken: cancellationToken);

        if (activeSchedule is null)
        {
            return Array.Empty<AvailableTimeDto>();
        }

        var slots = await _readOnlyRepository.GetSelected<AvailableSlot, AvailableTimeDto>(
            filter: s => s.ScheduleId == activeSchedule.Id && s.DayOfWeek == dayOfWeek && !s.IsReserved,
            selector: s => new AvailableTimeDto()
            {
                TimeSlotId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            },
            cancellationToken: cancellationToken);

        return slots.OrderBy(s => s.StartTime).ToArray();
    }

    /// <inheritdoc/>
    public async Task<bool> TryReserveSlot(long telegramUserId, DayOfWeek dayOfWeek, TimeSpan startTime,
        CancellationToken cancellationToken)
    {
        await using var tr = await _repository.CreateTransaction(cancellationToken);

        try
        {
            var activeSchedule = await _readOnlyRepository.GetFirstOrDefault<Data.Context.Entity.Schedule>(
                filter: s => s.IsActiveWeek,
                cancellationToken: cancellationToken);

            if (activeSchedule is null)
            {
                await tr.RollbackAsync(cancellationToken);
                return false;
            }

            // Находим точный слот
            var slot = await _readOnlyRepository.GetFirstOrDefault<AvailableSlot>(
                filter: s => s.ScheduleId == activeSchedule.Id
                             && s.DayOfWeek == dayOfWeek
                             && s.StartTime == startTime,
                cancellationToken: cancellationToken);

            if (slot is null || slot.IsReserved)
            {
                await tr.RollbackAsync(cancellationToken);
                return false;
            }

            await _repository.BatchUpdate<AvailableSlot>(
                filter: s => s.ScheduleId == activeSchedule.Id
                             && s.DayOfWeek == dayOfWeek
                             && s.StartTime == startTime,
                updateAction: props => props
                    .SetProperty(s => s.IsReserved, true)
                    .SetProperty(s => s.ModifiedDate, DateTime.UtcNow),
                cancellationToken: cancellationToken);

            var booking = new Booking(telegramUserId, 1, slot.Id);
            await _repository.Add(booking, tr, cancellationToken);

            await tr.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TryReserveSlot failed for user {User} {Day} {Time}", telegramUserId, dayOfWeek,
                startTime);
            await tr.RollbackAsync(cancellationToken);
            return false;
        }
    }

    /// <summary>
    /// Проверяет наличие активного расписания на текущую неделю.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если активное расписание существует, иначе false.</returns>
    private async Task<bool> HasActiveSchedule(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await _readOnlyRepository.IfExists<Data.Context.Entity.Schedule>(
            filter: s => now >= s.WeekStartDate && now <= s.WeekEndDate && s.IsActiveWeek,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Создает новое расписание и генерирует доступные временные слоты.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    private async Task CreateNewSchedule(CancellationToken cancellationToken)
    {
        await using var tr = await _repository.CreateTransaction(cancellationToken);

        try
        {
            await DeactivatingActiveSchedules(cancellationToken);
                
            var schedule = new Data.Context.Entity.Schedule(GetStartOfWeek(DateTime.Today), GetEndOfWeek(DateTime.Today), true);
            await _repository.Add(schedule, tr, cancellationToken);

            var availableSlot = CreateAvailableSlot(schedule.Id);
            await _repository.AddRange(availableSlot, tr, cancellationToken);

            await tr.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error to create NewSchedule: {ex.Message}");
            await tr.RollbackAsync(cancellationToken);
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
        int scheduleId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, TimeSpan interval)
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
    private AvailableSlot[] CreateAvailableSlot(int scheduleId)
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
        await _repository.BatchUpdate<Data.Context.Entity.Schedule>(
            filter: s => s.IsActiveWeek,
            updateAction: props => props
                .SetProperty(s => s.IsActiveWeek, false)
                .SetProperty(s => s.ModifiedDate, DateTime.UtcNow),
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