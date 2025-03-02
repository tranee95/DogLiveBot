using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class SettingsCommand : ICommand
{
    private readonly IKeyboardService _keyboardService;
    private readonly ITelegramBotClient _botClient;

    public SettingsCommand(
        IKeyboardService keyboardService, 
        ITelegramBotClient botClient)
    {
        _keyboardService = keyboardService;
        _botClient = botClient;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Settings;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Menu;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
         await _botClient.SendMessage(message.Chat.Id, MessageText.UserSettings, replyMarkup:
            _keyboardService.GetSettings(), cancellationToken: cancellationToken);

         if (callbackQuery != null)
         {
             await _botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
         }
    }
}