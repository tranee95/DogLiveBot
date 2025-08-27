using DogLiveBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandInterface;

public interface IReceivedTextCommand
{
    public CommandTypeEnum CommandType { get; }
    public CommandTypeEnum BackCommandType { get; }
    public Task ExecuteTextCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}