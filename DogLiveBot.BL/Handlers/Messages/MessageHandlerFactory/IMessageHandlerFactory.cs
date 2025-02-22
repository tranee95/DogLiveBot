using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Handlers.Messages.MessageHandlerFactory;

public interface IMessageHandlerFactory
{ 
    IMessageHandler GetMessageHandler(MessageType type);
}