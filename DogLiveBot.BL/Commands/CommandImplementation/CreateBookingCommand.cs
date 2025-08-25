using System.Globalization;
using System.Text.Json;
using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface.Cache;
using DogLiveBot.BL.Services.ServiceInterface.Keyboard;
using DogLiveBot.BL.Services.ServiceInterface.Schedule;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Models.CommadData;
using DogLiveBot.Data.Models.CommandData;
using DogLiveBot.Data.Models.Options;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using DogLiveBot.Data.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class CreateBookingCommand : CallbackQueryCommand, ICommand, IReceivedDataCommand
{
    private readonly ILogger<CreateBookingCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;
    private readonly IReadOnlyRepository _readOnlyRepository;
    private readonly IScheduleService _scheduleService;
    private readonly IBookingFlowCacheService _bookingFlowCacheService;
    private readonly IOptions<ApplicationOptions> _options;
    
    private readonly CultureInfo Ru = new("ru-RU");

    public CreateBookingCommand(
        IMapper mapper,
        ITelegramBotClient telegramBotClient,
        IRepository repository,
        IReadOnlyRepository readOnlyRepository,
        ILogger<CreateBookingCommand> logger,
        IKeyboardService keyboardService, 
        IScheduleService scheduleService,
        IBookingFlowCacheService bookingFlowCacheService, 
        IOptions<ApplicationOptions> options)
        : base(mapper, telegramBotClient, repository, readOnlyRepository)
    {
        _logger = logger;
        _botClient = telegramBotClient;
        _keyboardService = keyboardService;
        _scheduleService = scheduleService;
        _bookingFlowCacheService = bookingFlowCacheService;
        _options = options;
        _readOnlyRepository = readOnlyRepository;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.CreateBooking;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery)
    {
        var days = await _scheduleService.GetActiveDays(cancellationToken);
        await SendDaySelection(message.Chat.Id, days, cancellationToken);
    }

    public async Task ExecuteReceivedDataLogic(Message message, CommandData commandData,
        CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        BookingPayload? payload = null;
        try
        {
            payload = JsonSerializer.Deserialize<BookingPayload>(commandData.Data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize BookingPayload for user {UserId}", message.Chat.Id);
        }

        var telegramUserId = callbackQuery?.From?.Id ?? message.From?.Id ?? message.Chat.Id;

        if (payload is null)
        {
            await RestartFlow(message.Chat.Id, cancellationToken);
            return;
        }

        // выбран только день -> показать слоты времени
        if (payload.DayOfWeek != default && payload.TimeSlotId == default)
        {
            await HandleDaySelectedAsync(telegramUserId, message.Chat.Id, (DayOfWeek)payload.DayOfWeek, cancellationToken);
            return;
        }

        // выбран день и время -> пробуем забронировать
        if (payload.DayOfWeek != default && payload.TimeSlotId != default)
        {
            await HandleTimeSelectedAsync(telegramUserId, message.Chat.Id, (DayOfWeek)payload.DayOfWeek, payload.TimeSlotId, cancellationToken);
            return;
        }

        // Если ни день, ни время не определены — перезапуск
        await RestartFlow(message.Chat.Id, cancellationToken);
    }

    private async Task HandleDaySelectedAsync(long telegramUserId, long chatId, DayOfWeek day, CancellationToken cancellationToken)
    {
        //TODO ещё под вопросом данное решение
        var state = await _bookingFlowCacheService.GetAsync(telegramUserId, cancellationToken);
        state.DayOfWeek = day;
        state.Time = null;
        state.Date = null;

        var ttlMinutes = _options.Value.RedisSettings.LiveTimeMinutes;
        await _bookingFlowCacheService.SetAsync(telegramUserId, state, TimeSpan.FromMinutes(ttlMinutes), cancellationToken);

        var times = await _scheduleService.GetAvailableTimes(day, cancellationToken);
        if (times.Count == 0)
        {
            await _botClient.SendMessage(chatId, MessageText.ThereNoAvailableSlots, 
                cancellationToken: cancellationToken);

            var days = await _scheduleService.GetActiveDays(cancellationToken);
            await SendDaySelection(chatId, days, cancellationToken);
            return;
        }

        await SendTimeSelection(chatId, day, times, cancellationToken);
    }

    private async Task HandleTimeSelectedAsync(long telegramUserId, long chatId, DayOfWeek dayOfWeek, int timeSlotId, CancellationToken cancellationToken)
    {
        var timeSlot = await _readOnlyRepository.GetFirstOrDefault<AvailableSlot>(
            filter: s => s.Id == timeSlotId,
            cancellationToken: cancellationToken);

        if (timeSlot is null)
        {
            await RestartFlow(chatId, cancellationToken);
        }

        var reserved = await _scheduleService.TryReserveSlot(telegramUserId, dayOfWeek, timeSlot.StartTime, cancellationToken);
        if (reserved)
        {
            await _bookingFlowCacheService.ClearAsync(telegramUserId, cancellationToken);
            var timeText = timeSlot.StartTime.ToString(@"hh\:mm");

            var dayOfWeekRu = Ru.DateTimeFormat.GetDayName(dayOfWeek);
            await _botClient.SendMessage(
                chatId, string.Format(MessageText.BookingCreatedTemplate, dayOfWeekRu, timeText),
                cancellationToken: cancellationToken);
        }
        else
        {
            await RestartFlow(chatId, cancellationToken);
        }
    }

    private async Task RestartFlow(long chatId, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, MessageText.RestartBookingFlow,
            cancellationToken: cancellationToken);

        var days = await _scheduleService.GetActiveDays(cancellationToken);
        await SendDaySelection(chatId, days, cancellationToken);
    }

    private async Task SendDaySelection(long chatId, ICollection<DaysDto> days, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, MessageText.ChooseDay,
            replyMarkup: _keyboardService.GetDays(days), cancellationToken: cancellationToken);
    }

    private async Task SendTimeSelection(long chatId, DayOfWeek day, ICollection<AvailableTimeDto> times,
        CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, MessageText.ChooseTime,
            replyMarkup: _keyboardService.GetTimes(day, times),
            cancellationToken: cancellationToken);
    }
}