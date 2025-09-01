using DogLive.TelegramBot.BL.Handlers.MassTransitMessages.MassTransitMessageHandlerInterface;
using MassTransit;
using Shared.Messages.Messages.Schedule.Model;

namespace DogLive.TelegramBot.BL.Handlers.MassTransitMessages.Consumers;

public class ScheduleConsumer : IConsumer<IRMQScheduleSlot>
{
    private readonly IMassTransitMessageHandler<IRMQScheduleSlot> _handler;

    public ScheduleConsumer(
        IMassTransitMessageHandler<IRMQScheduleSlot> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<IRMQScheduleSlot> context)
    {
        using var cts = new CancellationTokenSource();
        await _handler.Handle(context.Message, cts.Token);
    }
}