using AutoMapper;
using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using DogLiveBot.Data.Model;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class DeleteDogCommand : CallbackQueryCommand, ICommand, IReceivedDataCommand
{
    private readonly ILogger<DeleteDogCommand> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;
    private readonly IRepository<Dog> _dogRepository;
    private readonly ICommandService _commandService;

    public DeleteDogCommand(
        ILogger<DeleteDogCommand> logger,
        IMapper mapper, 
        ITelegramBotClient botClient, 
        IRepository<UserCallbackQuery> userCallbackQueryRepository, 
        IKeyboardService keyboardService, 
        IRepository<Dog> dogRepository, 
        ICommandService commandService) 
        : base(mapper, botClient, userCallbackQueryRepository)
    {
        _logger = logger;
        _botClient = botClient;
        _keyboardService = keyboardService;
        _dogRepository = dogRepository;
        _commandService = commandService;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.DeleteDog;
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        var dogs = await _dogRepository.WhereSelected<DogDeleteModel>(
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
            if (!await _dogRepository.Delete(commandData.Id, cancellationToken))
            {
                throw new NullReferenceException("Dog not found");
            }

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