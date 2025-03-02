using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class AddDogCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;

    public AddDogCommand(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.AddDog;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SpecifyDogName, cancellationToken: cancellationToken);

        if (callbackQuery != null)
        {
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
        }
    }
}   