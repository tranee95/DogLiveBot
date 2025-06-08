using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DogLiveBot.BL.Commands.ReceivedDataCommandFactory;

public class ReceivedDataCommandFactory : IReceivedDataCommandFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReceivedDataCommandFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IReceivedDataCommand GetCommand(CommandTypeEnum? type)
    {
        var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<IReceivedDataCommand>();
        return commands.First(x => x.CommandType == type);
    }

    public IReceivedDataCommand GetBackCommand(CommandTypeEnum? type)
    {
        var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<IReceivedDataCommand>();
        var backCommandType = commands.First(x => x.CommandType == type).BackCommandType;

        return commands.First(x => x.CommandType == backCommandType);
    }
}