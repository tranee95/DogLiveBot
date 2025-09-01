using AutoMapper;
using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Cache;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Keyboard;
using DogLive.TelegramBot.Data.Enums;
using DogLive.TelegramBot.Data.Models;
using DogLive.TelegramBot.Data.Text;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DogLive.TelegramBot.Data.Context.Entity.User;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

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

    public override CommandTypeEnum BackCommandType => CommandTypeEnum.MainMenu;

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

        if (!string.IsNullOrWhiteSpace(cacheValue))
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