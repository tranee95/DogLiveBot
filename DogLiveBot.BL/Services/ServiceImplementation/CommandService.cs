using DogLiveBot.BL.Commands.CommandFactory;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums.Helpers;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Services.ServiceImplementation;

public class CommandService : ICommandService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IReadOnlyRepository _readOnlyRepository;
    private readonly ICommandFactory _commandFactory;

    public CommandService(
        ITelegramBotClient botClient,
        IReadOnlyRepository readOnlyRepository,
        ICommandFactory commandFactory)
    {
        _botClient = botClient;
        _readOnlyRepository = readOnlyRepository;
        _commandFactory = commandFactory;
    }

    /// <inheritdoc />
    public async Task ExecuteGoBackCommand(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        var userCallbackQuery = await _readOnlyRepository.GetFirstOrDefault<UserCallbackQuery>(
            filter: s => s.UserTelegramId == message.Chat.Id,
            cancellationToken: cancellationToken);

        if (userCallbackQuery == null)
        {
            throw new NullReferenceException("Back Commands is null");
        }

        var commandType = CommandTypeEnumHelper.GetCommandTypeEnum(userCallbackQuery.Data);

        var command = _commandFactory.GetBackCommand(commandType);
        await command.Execute(message, cancellationToken, callbackQuery);

        if (callbackQuery != null)
        {
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
        }
    }
}