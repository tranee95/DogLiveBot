using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Models.CommandData;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandInterface;

public interface IReceivedDataCommand
{
    public CommandTypeEnum CommandType { get; }
    public CommandTypeEnum BackCommandType { get; }
    public Task ExecuteReceivedDataLogic(Message message, CommandData commandData, 
        CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}