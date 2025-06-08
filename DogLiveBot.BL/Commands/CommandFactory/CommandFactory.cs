using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DogLiveBot.BL.Commands.CommandFactory;

public class CommandFactory : ICommandFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CommandFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public ICommand GetCommand(CommandTypeEnum? type)
    {
        var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<ICommand>();
        return commands.First(x => x.CommandType == type);
    }

    public ICommand GetBackCommand(CommandTypeEnum? type)
    {
        var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<ICommand>();
        var backCommandType = commands.First(x => x.CommandType == type).BackCommandType;

        return commands.First(x => x.CommandType == backCommandType);
    }
}