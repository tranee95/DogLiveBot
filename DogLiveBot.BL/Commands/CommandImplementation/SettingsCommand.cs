using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.BL.Services.ServiceInterface.Cache;
using DogLiveBot.BL.Services.ServiceInterface.Keyboard;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Models;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using DogLiveBot.Data.Text;
using Quartz.Util;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DogLiveBot.Data.Context.Entity.User;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class SettingsCommand : CallbackQueryCommand, ICommand
{
    private readonly IKeyboardService _keyboardService;
    private readonly ITelegramBotClient _botClient;
    private readonly IReadOnlyRepository _readOnlyRepository;
    private readonly ICacheService _cacheService;

    public SettingsCommand(
        IMapper mapper,
        ITelegramBotClient botClient,
        IRepository repository,
        IReadOnlyRepository readOnlyRepository,
        IKeyboardService keyboardService, 
        ICacheService cacheService) 
        : base(mapper, botClient, repository, readOnlyRepository)
    {
        _keyboardService = keyboardService;
        _readOnlyRepository = readOnlyRepository;
        _cacheService = cacheService;
        _botClient = botClient;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.Settings;

    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Menu;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        var settingsText = await PrepareSettingsMessage(message, cancellationToken);

        await _botClient.SendMessage(message.Chat.Id, settingsText, replyMarkup:
            _keyboardService.GetSettings(), cancellationToken: cancellationToken);
    }

    private async Task<string> PrepareSettingsMessage(Message message, CancellationToken cancellationToken)
    {
        var cacheKey = $"settings:{message.Chat.Id}";
        var cacheValue = await _cacheService.Get(cacheKey, cancellationToken);

        if (!cacheValue.IsNullOrWhiteSpace())
        {
            return cacheValue;
        }
        
        var settingsMessageText = await _readOnlyRepository.GetFirstOrDefaultSelected<User, SettingsMessageTextDto>(
            filter: s => s.TelegramId == message.Chat.Id,
            selector: s => new SettingsMessageTextDto
            {
                UserName = $"{s.FirstName}",
                DogNames = s.Dogs.Where(t => t.DeleteDate == null).Select(d => $" - {d.Name}").ToArray()
            },
            cancellationToken: cancellationToken);

        var dogsName = string.Join("\n", settingsMessageText.DogNames);
        var result = $"{MessageText.UserSettings}\n{settingsMessageText.UserName}\n\n{MessageText.MyDogs}:\n{dogsName}";

        await _cacheService.Set(cacheKey, result, cancellationToken);
        return result;
    }
}