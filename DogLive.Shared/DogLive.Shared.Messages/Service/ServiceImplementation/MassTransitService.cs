using System.Threading.Tasks;
using MassTransit;
using Shared.Messages.Messages.Service.ServiceInterface;

namespace Shared.Messages.Messages.Service.ServiceImplementation;

public class MassTransitService : IMassTransitService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish<TMessage>(TMessage message) where TMessage : class
    {
        await _publishEndpoint.Publish<TMessage>(message);
    }
}
