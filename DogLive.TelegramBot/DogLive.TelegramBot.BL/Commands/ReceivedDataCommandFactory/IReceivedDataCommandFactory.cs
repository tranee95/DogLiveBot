using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.Data.Enums;

namespace DogLive.TelegramBot.BL.Commands.ReceivedDataCommandFactory;

public interface IReceivedDataCommandFactory
{
    IReceivedDataCommand GetCommand(CommandTypeEnum? type);
    IReceivedDataCommand GetBackCommand(CommandTypeEnum? type);
}