using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface.Booking;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class MyNotesCommand : CallbackQueryCommand, ICommand
{
    private readonly ITelegramBotClient _botClient;
    private readonly IBookingService _bookingService;

    public MyNotesCommand(
        IMapper mapper,
        IRepository repository,
        IReadOnlyRepository readOnlyRepository,
        ITelegramBotClient botClient, 
        IBookingService bookingService) 
        : base(mapper, botClient, repository, readOnlyRepository)
    {
        _botClient = botClient;
        _bookingService = bookingService;
    }

    public override CommandTypeEnum CommandType => CommandTypeEnum.MyNotes;
    
    public override CommandTypeEnum BackCommandType => CommandTypeEnum.MainMenu;

    protected override async Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        var notes = await _bookingService.GeUserNotes(message.Chat.Id, cancellationToken);
        var text = string.Join(Environment.NewLine, notes.OrderBy(s => s.Date).ThenBy(s => s.Hour).Select(x => x.Text));

        await _botClient.SendMessage(message.Chat.Id, text, cancellationToken: cancellationToken);
    }
}