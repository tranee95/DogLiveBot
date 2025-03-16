using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Enums;

namespace DogLiveBot.BL.Command.ReceivedTextCommandFactory;

public interface IReceivedTextCommandFactory
{
    IReceivedTextCommand GetCommand(CommandTypeEnum? type);
    IReceivedTextCommand GetBackCommand(CommandTypeEnum? type);
}