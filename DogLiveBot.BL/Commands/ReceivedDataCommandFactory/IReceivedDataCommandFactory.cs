using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Enums;

namespace DogLiveBot.BL.Commands.ReceivedDataCommandFactory;

public interface IReceivedDataCommandFactory
{
    IReceivedDataCommand GetCommand(CommandTypeEnum? type);
    IReceivedDataCommand GetBackCommand(CommandTypeEnum? type);
}