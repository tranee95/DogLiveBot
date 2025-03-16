using DogLiveBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandInterface;

public interface IReceivedTextCommand
{
    public CommandTypeEnum CommandType { get; }
    public CommandTypeEnum BackCommandType { get; }
    
    public Task ExecuteReceivedTextLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}