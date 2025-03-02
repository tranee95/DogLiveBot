using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class MenuCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;

    public MenuCommand(
        IKeyboardService keyboardService,
        ITelegramBotClient botClient)
    {
        _keyboardService = keyboardService;
        _botClient = botClient;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Menu;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SelectAction,
            replyMarkup: _keyboardService.GetMainMenu(), cancellationToken: cancellationToken);

        if (callbackQuery != null)
        {
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
        }
    }
}