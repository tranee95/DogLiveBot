using System.Globalization;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Schedule;
using DogLive.TelegramBot.Data.Context.Entity;
using DogLive.TelegramBot.Data.Models;
using DogLive.TelegramBot.Data.Models.CommandData;
using Microsoft.Extensions.Logging;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;

namespace DogLive.TelegramBot.BL.Services.ServiceImplementation.Schedule;

/// <summary>
/// Сервис управления расписанием
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly IRepository _repository;
    private readonly IReadOnlyRepository _readOnlyRepository;

    public ScheduleService(
        ILogger<ScheduleService> logger,
        IRepository repository, 
        IReadOnlyRepository readOnlyRepository)
    {
        _logger = logger;
        _repository = repository;
        _readOnlyRepository = readOnlyRepository;
    }

    /// <inheritdoc/>
    public async Task FillCalendar(AvailableSlot[] availableSlots, CancellationToken cancellationToken)
    {
        if (await HasActiveSchedule(cancellationToken))
        {
            return;
        }

        await CreateNewSchedule(availableSlots, cancellationToken);
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
    public async Task<bool> TryReserveSlot(long telegramUserId, BookingPayload bookingPayload,
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
                             && s.DayOfWeek == (DayOfWeek)bookingPayload.DayOfWeek
                             && s.Id == bookingPayload.TimeSlotId,
                cancellationToken: cancellationToken);

            if (slot is null || slot.IsReserved)
            {
                await tr.RollbackAsync(cancellationToken);
                return false;
            }

            await _repository.BatchUpdate<AvailableSlot>(
                filter: s => s.ScheduleId == activeSchedule.Id
                             && s.DayOfWeek == (DayOfWeek)bookingPayload.DayOfWeek
                             && s.Id == bookingPayload.TimeSlotId,
                updateAction: props => props
                    .SetProperty(s => s.IsReserved, true)
                    .SetProperty(s => s.ModifiedDate, DateTime.UtcNow),
                cancellationToken: cancellationToken);

            var booking = new Data.Context.Entity.Booking(telegramUserId, bookingPayload.DogId, slot.Id);
            await _repository.Add(booking, tr, cancellationToken);

            await tr.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception)
        {
            await tr.RollbackAsync(cancellationToken);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<AvailableTimeDto> GetTimeSlotById(int timeSlotId, CancellationToken cancellationToken)
    {
        return await _readOnlyRepository.GetFirstOrDefaultSelected<AvailableSlot, AvailableTimeDto>(
            filter: s => s.Id == timeSlotId,
            selector: s => new AvailableTimeDto()
            {
                TimeSlotId = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            },
            cancellationToken: cancellationToken);
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
    private async Task CreateNewSchedule(AvailableSlot[] availableSlots, CancellationToken cancellationToken)
    {
        await using var tr = await _repository.CreateTransaction(cancellationToken);

        try
        {
            await DeactivatingActiveSchedules(cancellationToken);

            var schedule = new Data.Context.Entity.Schedule(GetStartOfWeek(DateTime.Today), GetEndOfWeek(DateTime.Today), true);
            await _repository.Add(schedule, tr, cancellationToken);

            foreach (var slot in availableSlots)
            {
                slot.ScheduleId = schedule.Id;
            }

            await _repository.AddRange<AvailableSlot>(availableSlots, tr, cancellationToken);

            await tr.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error to create NewSchedule: {ex.Message}");
            await tr.RollbackAsync(cancellationToken);
        }
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