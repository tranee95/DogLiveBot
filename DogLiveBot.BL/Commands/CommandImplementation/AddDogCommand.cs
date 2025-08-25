using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.BL.Services.ServiceInterface.Cache;
using DogLiveBot.BL.Services.ServiceInterface.Command;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using DogLiveBot.Data.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

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

    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    public async Task ExecuteReceivedTextLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
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

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SpecifyDogName, cancellationToken: cancellationToken);
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