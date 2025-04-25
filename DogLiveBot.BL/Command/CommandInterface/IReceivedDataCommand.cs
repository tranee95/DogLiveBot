using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Model;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandInterface;

public interface IReceivedDataCommand
{
    public CommandTypeEnum CommandType { get; }
    public CommandTypeEnum BackCommandType { get; }
    public Task ExecuteReceivedDataLogic(Message message, CommandData commandData, 
        CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}