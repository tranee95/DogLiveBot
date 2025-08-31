using AutoMapper;
using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Cache;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Command;
using DogLive.TelegramBot.Data.Context.Entity;
using DogLive.TelegramBot.Data.Enums;
using DogLive.TelegramBot.Data.Text;
using Microsoft.Extensions.Logging;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

public class AddDogCommand : CallbackQueryCommand, ICommand, IReceivedTextCommand
{
    private readonly ILogger<AddDogCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository _repository;
    private readonly ICommandService _commandService;
    private readonly ICacheService _cacheService;

    public AddDogCommand(
        ILogger<AddDogCommand> logger,
        IMapper mapper,
        ITelegramBotClient botClient,
        IRepository repository,
        IReadOnlyRepository readOnlyRepository, 
        ICommandService commandService, 
        ICacheService cacheService)
        : base(mapper, botClient, repository, readOnlyRepository)
    {
        _logger = logger;
        _botClient = botClient;
        _repository = repository;
        _commandService = commandService;
        _cacheService = cacheService;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.AddDog;

    public override CommandTypeEnum BackCommandType => CommandTypeEnum.MainMenu;

    public async Task ExecuteTextCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        await ExecuteTextCommand(message, cancellationToken, callbackQuery);
    }

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SpecifyDogName, cancellationToken: cancellationToken);
    }

    protected override async Task ExecuteTextCommandLogicCore(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(message.Text))
            {
                var dog = new Dog(message.Text, message.Chat.Id);

                await _repository.Add(dog, cancellationToken);
                await SendMessage(message, $"{MessageText.AddDogSuccess}", cancellationToken);

                var cacheKey = $"settings:{message.Chat.Id}";
                await _cacheService.Remove(cacheKey, cancellationToken);

                _logger.LogInformation($"Dog {dog.Name} was added for user {message.Chat.Id}");
            }
        }
        catch (Exception e)
        {
            await SendMessage(message, $"{MessageText.AddDogError}", cancellationToken);
            _logger.LogError($"Error to add dog {message.Chat.Id}, {nameof(AddDogCommand)}", e.Message);
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