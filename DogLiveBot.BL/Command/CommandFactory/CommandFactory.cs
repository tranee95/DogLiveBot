using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DogLiveBot.BL.Command.CommandFactory;

public class CommandFactory : ICommandFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CommandFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public ICommand GetCommand(CommandTypeEnum type)
    {
        var commands = _serviceScopeFactory.CreateScope().ServiceProvider.GetServices<ICommand>();
        return commands.First(x => x.CommandType == type);
    }
}