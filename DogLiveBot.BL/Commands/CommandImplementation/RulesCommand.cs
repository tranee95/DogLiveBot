using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.BL.Services.ServiceInterface.Keyboard;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class RulesCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;

    public RulesCommand(
        ITelegramBotClient botClient,
        IKeyboardService keyboardService)
    {
        _botClient = botClient;
        _keyboardService = keyboardService;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Rules;
    
    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.Rules, replyMarkup:
            _keyboardService.GetMainMenu(), cancellationToken: cancellationToken);
    }
}