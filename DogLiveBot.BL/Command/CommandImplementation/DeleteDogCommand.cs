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

namespace DogLiveBot.BL.Command.CommandImplementation;

public class DeleteDogCommand : CallbackQueryCommand, ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;
    private readonly IRepository<Dog> _dogRepository;


    
    public DeleteDogCommand(
        IMapper mapper, 
        ITelegramBotClient botClient, 
        IRepository<UserCallbackQuery> userCallbackQueryRepository, 
        IKeyboardService keyboardService, 
        IRepository<Dog> dogRepository) 
        : base(mapper, botClient, userCallbackQueryRepository)
    {
        _botClient = botClient;
        _keyboardService = keyboardService;
        _dogRepository = dogRepository;
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
}