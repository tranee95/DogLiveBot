using AutoMapper;
using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using DogLiveBot.Data.Model;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DogLiveBot.Data.Entity.User;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class SettingsCommand : CallbackQueryCommand, ICommand
{
    private readonly IKeyboardService _keyboardService;
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository<User> _userRepository;
    private readonly ICacheService _cacheService;

    public SettingsCommand(
        IMapper mapper,
        ITelegramBotClient botClient,
        IRepository<UserCallbackQuery> userCallbackQueryRepository,
        IKeyboardService keyboardService, 
        IRepository<User> userRepository, 
        ICacheService cacheService) 
        : base(mapper, botClient, userCallbackQueryRepository)
    {
        _keyboardService = keyboardService;
        _userRepository = userRepository;
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

        if (cacheValue != null)
        {
            return cacheValue;
        }

        var settingsMessageText = await _userRepository.GetFirstOrDefaultSelected<SettingsMessageTextModel>(
            filter: s => s.TelegramId == message.Chat.Id,
            selector: s => new SettingsMessageTextModel
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