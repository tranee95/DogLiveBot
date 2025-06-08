using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Enums;

namespace DogLiveBot.BL.Commands.ReceivedTextCommandFactory;

public interface IReceivedTextCommandFactory
{
    IReceivedTextCommand GetCommand(CommandTypeEnum? type);
    IReceivedTextCommand GetBackCommand(CommandTypeEnum? type);
}