using DogLiveBot.BL.Services.ServiceInterface.Booking;
using DogLiveBot.BL.Services.ServiceInterface.Schedule;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Models.Booking;
using DogLiveBot.Data.Models.CommandData;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using DogLiveBot.Data.Text;
using Microsoft.Extensions.Logging;

namespace DogLiveBot.BL.Services.ServiceImplementation.Booking;

public class BookingService : IBookingService
{
    private readonly IReadOnlyRepository _readOnlyRepository;
    private readonly IScheduleService _scheduleService;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IReadOnlyRepository readOnlyRepository,
        IScheduleService scheduleService,
        ILogger<BookingService> logger)
    {
        _readOnlyRepository = readOnlyRepository;
        _scheduleService = scheduleService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ICollection<DaysDto>> GetActiveDays(CancellationToken cancellationToken)
    {
        return await _scheduleService.GetActiveDays(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ICollection<AvailableTimeDto>> GetAvailableTimes(DayOfWeek day,
        CancellationToken cancellationToken)
    {
        return await _scheduleService.GetAvailableTimes(day, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AvailableSlot?> GetTimeSlot(int timeSlotId, CancellationToken cancellationToken)
    {
        return await _readOnlyRepository.GetFirstOrDefault<AvailableSlot>(
            filter: s => s.Id == timeSlotId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(bool IsNeedSelectDog, ICollection<DogDto> Dogs)> CheckIfNeedSelectDog(long telegramUserId,
        CancellationToken cancellationToken)
    {
        var dogs = await _readOnlyRepository.GetFirstOrDefaultSelected<Data.Context.Entity.User, ICollection<DogDto>>(
            filter: s => s.TelegramId == telegramUserId,
            selector: s => (s.Dogs.Select(t => new DogDto
            {
                Id = t.Id, 
                Name = t.Name
            })).ToArray(),
            cancellationToken: cancellationToken);

        if (dogs == null || !dogs.Any())
        {
            return (false, new List<DogDto>());
        }

        return (dogs.Count > 1, dogs);
    }

    /// <inheritdoc />
    public Task TryReserveSlot(long telegramUserId, BookingPayload payload, CancellationToken cancellationToken)
    {
        return _scheduleService.TryReserveSlot(telegramUserId, payload, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BookingStepResult> DetermineNextStep(BookingPayload payload, long telegramUserId,
        CancellationToken cancellationToken)
    {
        // Ничего не выбрано -> рестарт
        if (payload.IsDayOfWeekDefault() && payload.IsTimeSlotIdDefault() && payload.IsDogIdDefault())
        {
            return new BookingStepResult(BookingStepEnum.Restart);
        }

        // Только день выбран -> показать времена
        if (!payload.IsDayOfWeekDefault() && payload.IsTimeSlotIdDefault())
        {
            var times = await GetAvailableTimes((DayOfWeek)payload.DayOfWeek, cancellationToken);
            if (times.Count == 0)
            {
                return new BookingStepResult
                {
                    Step = BookingStepEnum.SelectDay,
                    ErrorMessage = MessageText.ThereNoAvailableSlots
                };
            }

            return new BookingStepResult(BookingStepEnum.SelectTime, times);
        }

        // День и время выбраны -> проверить слот и обработать выбор собаки
        if (!payload.IsDayOfWeekDefault() && !payload.IsTimeSlotIdDefault() && payload.IsDogIdDefault())
        {
            var timeSlot = await GetTimeSlot(payload.TimeSlotId, cancellationToken);
            if (timeSlot == null)
            {
                return new BookingStepResult(BookingStepEnum.Restart);
            }

            if (payload.IsDogIdDefault())
            {
                var (isNeedSelectDog, dogs) = await CheckIfNeedSelectDog(telegramUserId, cancellationToken);
                if (isNeedSelectDog)
                {
                    return new BookingStepResult(BookingStepEnum.SelectDog, dogs);
                }

                if (dogs.Any())
                {
                    payload.DogId = dogs.First().Id;
                }
            }

            // Если все заполнено (включая dog), резервируем
            if (!payload.IsDayOfWeekDefault() && !payload.IsTimeSlotIdDefault() && !payload.IsDogIdDefault())
            {
                return new BookingStepResult { Step = BookingStepEnum.Reserve };
            }
        }

        _logger.LogWarning("Incomplete payload for reservation: {@Payload}", payload);
        return new BookingStepResult { Step = BookingStepEnum.Restart };
    }
}