using AutoMapper;
using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DogLiveBot.Data.Entity.User;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class RenameCommand : CallbackQueryCommand, ICommand, IReceivedTextCommand
{
    private readonly ILogger<RenameCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository<User> _userRepository;
    private readonly ICommandService _commandService;
    
    public RenameCommand(
        IMapper mapper, 
        IRepository<UserCallbackQuery> userCallbackQueryRepository,
        ILogger<RenameCommand> logger, 
        ITelegramBotClient botClient, 
        IRepository<User> userRepository, 
        ICommandService commandService) 
        : base(mapper, botClient, userCallbackQueryRepository)
    {
        _logger = logger;
        _botClient = botClient;
        _userRepository = userRepository;
        _commandService = commandService;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.Rename;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    public async Task ExecuteReceivedTextLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                var user = await _userRepository.Get(s => s.TelegramId == message.Chat.Id, cancellationToken);
                if (user != null)
                {
                    user.FirstName = message.Text.Trim();

                    await _userRepository.Update(user, cancellationToken);
                    await SendMessage(message, MessageText.UserHasRename, cancellationToken);

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

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SetUserName, cancellationToken: cancellationToken);
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