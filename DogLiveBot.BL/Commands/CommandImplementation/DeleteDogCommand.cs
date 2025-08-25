using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface.Cache;
using DogLiveBot.BL.Services.ServiceInterface.Command;
using DogLiveBot.BL.Services.ServiceInterface.Keyboard;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Models.CommandData;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using DogLiveBot.Data.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class DeleteDogCommand : CallbackQueryCommand, ICommand, IReceivedDataCommand
{
    private readonly ILogger<DeleteDogCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;
    private readonly IReadOnlyRepository _readOnlyRepository;
    private readonly IRepository _repository;
    private readonly ICommandService _commandService;
    private readonly ICacheService _cacheService;

    public DeleteDogCommand(
        ILogger<DeleteDogCommand> logger,
        IMapper mapper, 
        ITelegramBotClient botClient, 
        IRepository repository,
        IReadOnlyRepository readOnlyRepository, 
        IKeyboardService keyboardService, 
        ICommandService commandService, 
        ICacheService cacheService) 
        : base(mapper, botClient, repository, readOnlyRepository)
    {
        _logger = logger;
        _botClient = botClient;
        _repository = repository;
        _keyboardService = keyboardService;
        _readOnlyRepository = readOnlyRepository;
        _commandService = commandService;
        _cacheService = cacheService;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.DeleteDog;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        var dogs = await _readOnlyRepository.GetSelected<Dog, DogDeleteDto>(
            filter: s => s.UserTelegramId == message.Chat.Id,
            selector: s => new DogDeleteDto()
            {
                Id = s.Id,
                Name = s.Name
            },
            cancellationToken: cancellationToken);

        await _botClient.SendMessage(message.Chat.Id, MessageText.ChooseDog,
            replyMarkup: _keyboardService.GetDeleteDogs(dogs), cancellationToken: cancellationToken);
    }

    public async Task ExecuteReceivedDataLogic(Message message, CommandData commandData, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        try
        {
            var dogId = commandData.GetData<int>();
            if (!await _repository.Delete<Dog>(dogId, cancellationToken))
            {
                throw new NullReferenceException("Dog not found");
            }

            var cacheKey = $"settings:{message.Chat.Id}";
            await _cacheService.Remove(cacheKey, cancellationToken);

            await SendMessage(message, $"{MessageText.DeleteDogSuccess}", cancellationToken);
        } 
        catch (NullReferenceException e)
        {
            await SendMessage(message, $"{MessageText.DeleteDogError}", cancellationToken);
            _logger.LogError($"Dog not found {message.Chat.Id}, {nameof(AddDogCommand)}", e.Message);
        }
        catch (Exception e)
        {
            await SendMessage(message, $"{MessageText.DeleteDogError}", cancellationToken);
            _logger.LogError($"Error to delete dog {message.Chat.Id}, {nameof(AddDogCommand)}", e.Message);
        }
        finally
        {
            await GoBack(message, cancellationToken, callbackQuery);
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