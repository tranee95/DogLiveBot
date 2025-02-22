using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Enums;

namespace DogLiveBot.BL.Command.CommandFactory;

public interface ICommandFactory
{
    ICommand GetCommand(CommandTypeEnum type);
}