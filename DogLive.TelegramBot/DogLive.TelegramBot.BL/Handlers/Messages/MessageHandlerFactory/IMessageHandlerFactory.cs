using DogLive.TelegramBot.BL.Handlers.Messages.MessageHandlerInterface;
using Telegram.Bot.Types.Enums;

namespace DogLive.TelegramBot.BL.Handlers.Messages.MessageHandlerFactory;

public interface IMessageHandlerFactory
{ 
    IMessageHandler GetMessageHandler(MessageType type);
}