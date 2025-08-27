using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface.Cache;
using DogLiveBot.BL.Services.ServiceInterface.Command;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using DogLiveBot.Data.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DogLiveBot.Data.Context.Entity.User;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class RenameCommand : CallbackQueryCommand, ICommand, IReceivedTextCommand
{
    private readonly ILogger<RenameCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository _repository;
    private readonly IReadOnlyRepository _readOnlyRepository;
    private readonly ICommandService _commandService;
    private readonly ICacheService _cacheService;

    public RenameCommand(
        IMapper mapper,
        ILogger<RenameCommand> logger,
        ITelegramBotClient botClient,
        IReadOnlyRepository readOnlyRepository,
        IRepository repository,
        ICommandService commandService,
        ICacheService cacheService)
        : base(mapper, botClient, repository, readOnlyRepository)
    {
        _logger = logger;
        _botClient = botClient;
        _readOnlyRepository = readOnlyRepository;
        _commandService = commandService;
        _cacheService = cacheService;
        _repository = repository;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.Rename;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    public async Task ExecuteTextCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        await ExecuteTextCommand(message, cancellationToken, callbackQuery);
    }

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SetUserName, cancellationToken: cancellationToken);
    }

    protected override async Task ExecuteTextCommandLogicCore(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                var user = await _readOnlyRepository.GetFirstOrDefault<User>(
                    filter: s => s.TelegramId == message.Chat.Id,
                    cancellationToken: cancellationToken);

                if (user != null)
                {
                    user.FirstName = message.Text.Trim();

                    await _repository.Update(user, cancellationToken);
                    await SendMessage(message, MessageText.UserHasRename, cancellationToken);

                    var cacheKey = $"settings:{message.Chat.Id}";
                    await _cacheService.Remove(cacheKey, cancellationToken);

                    _logger.LogInformation($"User {user.FirstName} was renamed TelegramId: {message.Chat.Id}");
                }
                else
                {
                    _logger.LogInformation($"User not found TelegramId: {message.Chat.Id}");
                }
            }
        }
        catch (Exception e)
        {
            await SendMessage(message, $"{MessageText.RenameUserError}", cancellationToken);
            _logger.LogError($"Error in {nameof(RenameCommand)} TelegramId: {message.Chat.Id}: {e.Message}");
        }
        finally
        {
            await GoBack(message, cancellationToken);
        }
    }

    private async Task SendMessage(Message message, string messageText, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(message.Chat.Id, messageText, cancellationToken: cancellationToken);
    }

    private async Task GoBack(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await _commandService.ExecuteGoBackCommand(message, cancellationToken, callbackQuery);
    }
}