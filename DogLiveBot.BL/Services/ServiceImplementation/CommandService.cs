using DogLiveBot.BL.Command.CommandFactory;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Extensions;
using DogLiveBot.Data.Enums.Helpers;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Services.ServiceImplementation;

public class CommandService : ICommandService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository<UserCallbackQuery> _userCallbackQueryRepository;
    private readonly ICommandFactory _commandFactory;

    public CommandService(
        ITelegramBotClient botClient,
        IRepository<UserCallbackQuery> userCallbackQueryRepository,
        ICommandFactory commandFactory)
    {
        _botClient = botClient;
        _userCallbackQueryRepository = userCallbackQueryRepository;
        _commandFactory = commandFactory;
    }

    /// <inheritdoc />
    public async Task ExecuteGoBackCommand(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        var userCallbackQuery = await _userCallbackQueryRepository.GetFirstOrDefault(s => s.UserTelegramId == message.Chat.Id, cancellationToken);

        if (userCallbackQuery == null)
        {
            throw new NullReferenceException("Back Command is null");
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