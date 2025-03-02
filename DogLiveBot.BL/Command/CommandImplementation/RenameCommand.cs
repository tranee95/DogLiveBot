using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class RenameCommand : ICommand
{
    public CommandTypeEnum CommandType => CommandTypeEnum.Rename;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        throw new NotImplementedException();
    }
}