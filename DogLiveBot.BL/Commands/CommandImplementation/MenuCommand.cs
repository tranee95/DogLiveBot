using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.BL.Services.ServiceInterface.Keyboard;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using DogLiveBot.Data.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

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

    public override CommandTypeEnum CommandType => CommandTypeEnum.Menu;

    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SelectAction,
            replyMarkup: _keyboardService.GetMainMenu(), cancellationToken: cancellationToken);
    }
}