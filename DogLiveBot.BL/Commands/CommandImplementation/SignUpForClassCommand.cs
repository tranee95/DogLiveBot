using System.Globalization;
using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class SignUpForClassCommand : CallbackQueryCommand, ICommand, IReceivedDataCommand
{
    private readonly ILogger<DeleteDogCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;
    private readonly IRepository<Schedule> _scheduleRepository;

    public SignUpForClassCommand(
        IMapper mapper,
        ITelegramBotClient telegramBotClient,
        IRepository<UserCallbackQuery> userCallbackQueryRepository,
        ILogger<DeleteDogCommand> logger,
        ITelegramBotClient botClient,
        IKeyboardService keyboardService,
        IRepository<Schedule> scheduleRepository)
        : base(mapper, telegramBotClient, userCallbackQueryRepository)
    {
        _logger = logger;
        _botClient = botClient;
        _keyboardService = keyboardService;
        _scheduleRepository = scheduleRepository;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.SignUpForClass;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery)
    {
        var activeSchedule = await _scheduleRepository.GetFirstOrDefault(s => s.IsActiveWeek, cancellationToken);

        var days = new List<DaysModel>();
        for (var date = activeSchedule.WeekStartDate; date <= activeSchedule.WeekEndDate; date = date.AddDays(1))
        {
            if (date >= DateTime.Now)
            {
                days.Add(new DaysModel() { 
                    Text = $"{date.ToShortDateString()} ({date.ToString("dddd", new CultureInfo("ru-RU"))})",
                    DayOfWeek = date.DayOfWeek
                });
            }
        }

        await _botClient.SendMessage(message.Chat.Id, MessageText.ChooseDay,
            replyMarkup: _keyboardService.GetDays(days), cancellationToken: cancellationToken);
    }

    public async Task ExecuteReceivedDataLogic(Message message, CommandData commandData,
        CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        throw new NotImplementedException();
    }
}