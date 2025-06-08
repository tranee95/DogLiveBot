using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DogLiveBot.BL.Commands.ReceivedTextCommandFactory;

public class ReceivedTextCommandFactory : IReceivedTextCommandFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ReceivedTextCommandFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IReceivedTextCommand GetCommand(CommandTypeEnum? type)
    {
        var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<IReceivedTextCommand>();
        return commands.First(x => x.CommandType == type);
    }

    public IReceivedTextCommand GetBackCommand(CommandTypeEnum? type)
    {
        var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<IReceivedTextCommand>();
        var backCommandType = commands.First(x => x.CommandType == type).BackCommandType;

        return commands.First(x => x.CommandType == backCommandType);
    }
}