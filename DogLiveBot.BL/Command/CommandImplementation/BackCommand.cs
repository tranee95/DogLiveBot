using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class BackCommand : ICommand
{
    private readonly ICommandService _commandService;

    public BackCommand(
        ICommandService commandService)
    {
        _commandService = commandService;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Back;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await _commandService.ExecuteGoBackCommand(message, cancellationToken, callbackQuery);
    }
}