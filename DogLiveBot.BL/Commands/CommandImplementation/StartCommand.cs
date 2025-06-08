using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class StartCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;

    public StartCommand(
        ITelegramBotClient botClient, 
        IKeyboardService keyboardService)
    {
        _botClient = botClient;
        _keyboardService = keyboardService;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Start;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.CompleteShortRegistration,
            replyMarkup: _keyboardService.GetCompleteShortRegistrationMenu(), cancellationToken: cancellationToken);
    }
}