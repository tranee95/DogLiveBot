using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.Data.Enums;

namespace DogLive.TelegramBot.BL.Commands.ReceivedTextCommandFactory;

public interface IReceivedTextCommandFactory
{
    IReceivedTextCommand GetCommand(CommandTypeEnum? type);
    IReceivedTextCommand GetBackCommand(CommandTypeEnum? type);
}