using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Keyboard;
using DogLive.TelegramBot.BL.Services.ServiceInterface;
using DogLive.TelegramBot.Data.Enums;
using DogLive.TelegramBot.Data.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

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