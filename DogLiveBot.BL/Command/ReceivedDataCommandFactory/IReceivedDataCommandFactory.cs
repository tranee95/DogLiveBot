using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Enums;

namespace DogLiveBot.BL.Command.ReceivedDataCommandFactory;

public interface IReceivedDataCommandFactory
{
    IReceivedDataCommand GetCommand(CommandTypeEnum? type);
    IReceivedDataCommand GetBackCommand(CommandTypeEnum? type);
}