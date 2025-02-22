using DogLiveBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandInterface;

public interface ICommand
{
    public CommandTypeEnum CommandType { get; }
    
    public Task Execute(Message message, CancellationToken cancellationToken);
}