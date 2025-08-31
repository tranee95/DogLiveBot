using System.Globalization;
using System.Text.Json;
using AutoMapper;
using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Booking;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Command;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Keyboard;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Schedule;
using DogLive.TelegramBot.Data.Enums;
using DogLive.TelegramBot.Data.Models;
using DogLive.TelegramBot.Data.Models.Booking;
using DogLive.TelegramBot.Data.Models.CommandData;
using DogLive.TelegramBot.Data.Text;
using Microsoft.Extensions.Logging;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

public class CreateBookingCommand : CallbackQueryCommand, ICommand, IReceivedDataCommand
{
    private readonly ILogger<CreateBookingCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;
    private readonly IScheduleService _scheduleService;
    private readonly ICommandService _commandService;
    private readonly IBookingService _bookingService;

    private readonly CultureInfo Ru = new("ru-RU");

    public CreateBookingCommand(
        IMapper mapper,
        ITelegramBotClient telegramBotClient,
        IRepository repository,
        IReadOnlyRepository readOnlyRepository,
        ILogger<CreateBookingCommand> logger,
        IKeyboardService keyboardService,
        IScheduleService scheduleService,
        ICommandService commandService,
        IBookingService bookingService)
        : base(mapper, telegramBotClient, repository, readOnlyRepository)
    {
        _logger = logger;
        _botClient = telegramBotClient;
        _keyboardService = keyboardService;
        _scheduleService = scheduleService;
        _commandService = commandService;
        _bookingService = bookingService;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.CreateBooking;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.MainMenu;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery)
    {
        var days = await _scheduleService.GetActiveDays(cancellationToken);
        await SendDaySelection(message.Chat.Id, days, cancellationToken);
    }

    public async Task ExecuteReceivedDataLogic(Message message, CommandData commandData,
        CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await ExecuteDataCommand(message, commandData, cancellationToken, callbackQuery);
    }

    protected override async Task ExecuteDataCommandLogicCore(Message message, CommandData commandData,
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

        if (payload == null)
        {
            await RestartFlow(message.Chat.Id, cancellationToken);
            return;
        }

        try
        {
            var stepResult = await _bookingService.DetermineNextStep(payload, telegramUserId, cancellationToken);
            switch (stepResult.Step)
            {
                case BookingStepEnum.SelectDay:
                    await HandleSelectDayAsync(message.Chat.Id, stepResult, cancellationToken); break;
                case BookingStepEnum.SelectTime:
                    await HandleSelectTimeAsync(message.Chat.Id, payload, stepResult, cancellationToken); break;
                case BookingStepEnum.SelectDog:
                    await HandleSelectDogAsync(message.Chat.Id, payload, stepResult, cancellationToken); break;
                case BookingStepEnum.Reserve:
                    await HandleReserveAsync(message, telegramUserId, payload, cancellationToken, callbackQuery); break;
                case BookingStepEnum.Restart:
                    await RestartFlow(message.Chat.Id, cancellationToken); break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process BookingPayload for user {UserId}", message.Chat.Id);
            await RestartFlow(message.Chat.Id, cancellationToken);
        }
    }

    private async Task RestartFlow(long chatId, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, MessageText.RestartBookingFlow, cancellationToken: cancellationToken);
        var days = await _bookingService.GetActiveDays(cancellationToken);
        await SendDaySelection(chatId, days, cancellationToken);
    }

    private async Task SendDaySelection(long chatId, ICollection<DaysDto> days, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, MessageText.ChooseDay,
            replyMarkup: _keyboardService.GetDays(days), cancellationToken: cancellationToken);
    }

    private async Task HandleSelectDayAsync(long chatId, BookingStepResult stepResult,
        CancellationToken cancellationToken)
    {
        var days = await _bookingService.GetActiveDays(cancellationToken);
        if (!string.IsNullOrEmpty(stepResult.ErrorMessage))
        {
            await _botClient.SendMessage(chatId, stepResult.ErrorMessage, cancellationToken: cancellationToken);
        }

        await SendDaySelection(chatId, days, cancellationToken);
    }

    private async Task HandleSelectTimeAsync(long chatId, BookingPayload payload, BookingStepResult stepResult,
        CancellationToken cancellationToken)
    {
        var availableTimes = (ICollection<AvailableTimeDto>)stepResult.Data!;
        await SendTimeSelection(chatId, (DayOfWeek)payload.DayOfWeek, availableTimes, cancellationToken);
    }

    private async Task HandleSelectDogAsync(long chatId, BookingPayload payload, BookingStepResult stepResult,
        CancellationToken cancellationToken)
    {
        var dogs = (ICollection<DogDto>)stepResult.Data!;
        await SendDogSelection(chatId, (DayOfWeek)payload.DayOfWeek, payload.TimeSlotId, dogs, cancellationToken);
    }

    private async Task HandleReserveAsync(Message message, long telegramUserId, BookingPayload payload,
        CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        await _bookingService.TryReserveSlot(telegramUserId, payload, cancellationToken);
        await SendConfirmation(message.Chat.Id, (DayOfWeek)payload.DayOfWeek, payload.TimeSlotId, cancellationToken);
        await GoBack(message, cancellationToken, callbackQuery);
    }

    private async Task SendTimeSelection(long chatId, DayOfWeek day, ICollection<AvailableTimeDto> times,
        CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, MessageText.ChooseTime,
            replyMarkup: _keyboardService.GetTimes(day, times),
            cancellationToken: cancellationToken);
    }

    private async Task SendDogSelection(long chatId, DayOfWeek day, int timeSlotId, ICollection<DogDto> dogs,
        CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, MessageText.ChooseDog,
            replyMarkup: _keyboardService.GetDogs(day, timeSlotId, dogs), cancellationToken: cancellationToken);
    }

    private async Task SendConfirmation(long chatId, DayOfWeek dayOfWeek, int timeSlotId,
        CancellationToken cancellationToken)
    {
        var timeSlot = await _scheduleService.GetTimeSlotById(timeSlotId, cancellationToken);
        var timeText = timeSlot.StartTime.ToString(@"hh\:mm");

        var dayOfWeekRu = Ru.DateTimeFormat.GetDayName(dayOfWeek);
        await _botClient.SendMessage(chatId, string.Format(MessageText.BookingCreatedTemplate, dayOfWeekRu, timeText),
            cancellationToken: cancellationToken);
    }

    private async Task GoBack(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await _commandService.ExecuteGoBackCommand(message, cancellationToken, callbackQuery);
    }
}