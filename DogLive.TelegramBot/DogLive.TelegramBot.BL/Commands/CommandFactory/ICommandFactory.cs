using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.Data.Enums;

namespace DogLive.TelegramBot.BL.Commands.CommandFactory;

public interface ICommandFactory
{
    ICommand GetCommand(CommandTypeEnum? type);
    ICommand GetBackCommand(CommandTypeEnum? type);
}