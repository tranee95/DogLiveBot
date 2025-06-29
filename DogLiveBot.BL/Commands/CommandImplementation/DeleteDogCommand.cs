using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
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
    private readonly IChangeRepository _changeRepository;
    private readonly ICommandService _commandService;
    private readonly ICacheService _cacheService;

    public DeleteDogCommand(
        ILogger<DeleteDogCommand> logger,
        IMapper mapper, 
        ITelegramBotClient botClient, 
        IChangeRepository changeRepository,
        IReadOnlyRepository readOnlyRepository, 
        IKeyboardService keyboardService, 
        ICommandService commandService, 
        ICacheService cacheService) 
        : base(mapper, botClient, changeRepository, readOnlyRepository)
    {
        _logger = logger;
        _botClient = botClient;
        _changeRepository = changeRepository;
        _keyboardService = keyboardService;
        _readOnlyRepository = readOnlyRepository;
        _commandService = commandService;
        _cacheService = cacheService;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.DeleteDog;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        var dogs = await _readOnlyRepository.WhereSelected<Dog, DogDeleteModel>(
            filter: s => s.UserTelegramId == message.Chat.Id,
            selector: s => new DogDeleteModel()
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
            if (!await _changeRepository.Delete<Dog>(commandData.Id, cancellationToken))
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