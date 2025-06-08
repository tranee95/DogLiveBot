using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Enums;

namespace DogLiveBot.BL.Commands.CommandFactory;

public interface ICommandFactory
{
    ICommand GetCommand(CommandTypeEnum? type);
    ICommand GetBackCommand(CommandTypeEnum? type);
}