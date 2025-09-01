namespace DogLive.TelegramBot.BL.Handlers.MassTransitMessages.MassTransitMessageHandlerInterface;

public interface IMassTransitMessageHandler<TMessage> where TMessage : class
{
    Task Handle(TMessage message, CancellationToken cancellation);
}