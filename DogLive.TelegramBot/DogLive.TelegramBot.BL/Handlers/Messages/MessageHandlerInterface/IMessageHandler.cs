using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DogLive.TelegramBot.BL.Handlers.Messages.MessageHandlerInterface;

public interface IMessageHandler
{
    public MessageType MessageType { get; }
    
    public Task HandleMessage(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null);
}