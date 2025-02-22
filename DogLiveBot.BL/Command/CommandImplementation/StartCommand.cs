using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class StartCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;

    public StartCommand(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Start;

    public async Task Execute(Message message, CancellationToken cancellationToken)
    {
        var replyMarkup = new ReplyKeyboardMarkup(new[]
        {
            KeyboardButton.WithRequestContact(ButtonText.SendPhoneNumber)
        })
        {
            ResizeKeyboard = true,
        };

        await _botClient.SendMessage(message.Chat.Id, MessageText.CompleteShortRegistration,
            replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }
}