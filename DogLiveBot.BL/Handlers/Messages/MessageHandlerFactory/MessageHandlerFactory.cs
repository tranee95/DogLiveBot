using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Handlers.Messages.MessageHandlerFactory;

public class MessageHandlerFactory : IMessageHandlerFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MessageHandlerFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IMessageHandler GetMessageHandler(MessageType type)
    {
       var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<IMessageHandler>();
       return commands.First(x => x.MessageType == type);
    }
}