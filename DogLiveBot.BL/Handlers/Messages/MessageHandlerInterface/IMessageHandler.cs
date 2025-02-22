using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;

public interface IMessageHandler
{
    public MessageType MessageType { get; }
    
    public Task HandleMessage(Message message, CancellationToken cancellationToken);
}