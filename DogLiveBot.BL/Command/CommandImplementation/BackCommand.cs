using DogLiveBot.BL.Command.CommandFactory;
using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Helpers;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class BackCommand : ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository<UserCallbackQuery> _userCallbackQueryRepository; 
    private readonly ICommandFactory _commandFactory;

    public BackCommand(
        ITelegramBotClient botClient, 
        IRepository<UserCallbackQuery> userCallbackQueryRepository, 
        ICommandFactory commandFactory)
    {
        _botClient = botClient;
        _userCallbackQueryRepository = userCallbackQueryRepository;
        _commandFactory = commandFactory;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Back;

    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        var userCallbackQuery = await _userCallbackQueryRepository.Get(s => s.UserTelegramId == message.Chat.Id, cancellationToken);
        var commandType = CommandTypeEnumHelper.GetCommandTypeEnum(userCallbackQuery.Data);

        var command = _commandFactory.GetBackCommand(commandType);
        await command.Execute(message, cancellationToken, callbackQuery);

        if (callbackQuery != null)
        {
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
        }
    }
}