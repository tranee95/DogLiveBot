using AutoMapper;
using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Keyboard;
using DogLive.TelegramBot.Data.Enums;
using DogLive.TelegramBot.Data.Text;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

public class MenuCommand : CallbackQueryCommand, ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;

    public MenuCommand(
        IMapper mapper,
        ITelegramBotClient botClient,
        IRepository repository,
        IReadOnlyRepository readOnlyRepository,
        IKeyboardService keyboardService) 
        : base(mapper, botClient, repository, readOnlyRepository)
    {
        _keyboardService = keyboardService;
        _botClient = botClient;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.MainMenu;

    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SelectAction,
            replyMarkup: _keyboardService.GetMainMenu(), cancellationToken: cancellationToken);
    }
}