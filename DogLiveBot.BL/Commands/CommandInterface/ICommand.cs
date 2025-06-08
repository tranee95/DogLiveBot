using DogLiveBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandInterface;

public interface ICommand
{
    public CommandTypeEnum CommandType { get; }
    
    public CommandTypeEnum BackCommandType { get; }
    
    public Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}