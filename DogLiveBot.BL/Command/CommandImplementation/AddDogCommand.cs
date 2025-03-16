using AutoMapper;
using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Menu;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Command.CommandImplementation;

public class AddDogCommand : CallbackQueryCommand, ICommand, IReceivedTextCommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository<Dog> _dogRepository;

    public AddDogCommand(
        IMapper mapper,
        ITelegramBotClient botClient,
        IRepository<Dog> dogRepository,
        IRepository<UserCallbackQuery?> userCallbackQueryRepository)
        : base(mapper, botClient, userCallbackQueryRepository)
    {
        _botClient = botClient;
        _dogRepository = dogRepository;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.AddDog;

    public override CommandTypeEnum BackCommandType => CommandTypeEnum.Settings;

    public async Task ExecuteReceivedTextLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        if (!string.IsNullOrWhiteSpace(message.Text))
        {
            var dog = new Dog
            {
                Name = message.Text,
                UserTelegramId = message.Chat.Id,
            };

            await _dogRepository.Add(dog, cancellationToken);
        }
    }

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery)
    {
        await _botClient.SendMessage(message.Chat.Id, MessageText.SpecifyDogName, cancellationToken: cancellationToken);
    }
}