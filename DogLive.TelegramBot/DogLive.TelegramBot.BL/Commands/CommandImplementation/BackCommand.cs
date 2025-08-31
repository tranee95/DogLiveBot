using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Command;
using DogLive.TelegramBot.BL.Services.ServiceInterface;
using DogLive.TelegramBot.Data.Enums;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

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