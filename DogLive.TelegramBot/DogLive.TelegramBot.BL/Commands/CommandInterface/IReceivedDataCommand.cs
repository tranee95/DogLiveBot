using DogLive.TelegramBot.Data.Enums;
using DogLive.TelegramBot.Data.Models;
using DogLive.TelegramBot.Data.Models.CommandData;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandInterface;

public interface IReceivedDataCommand
{
    public CommandTypeEnum CommandType { get; }
    public CommandTypeEnum BackCommandType { get; }
    public Task ExecuteReceivedDataLogic(Message message, CommandData commandData, 
        CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}