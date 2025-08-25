using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.BL.Services.ServiceInterface.Keyboard;
using DogLiveBot.BL.Services.ServiceInterface.User;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using User = DogLiveBot.Data.Context.Entity.User;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public class RegistrationCommand : ICommand
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Contact, User>()
                .ForMember(dest => dest.TelegramId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
    
    private readonly ILogger<RegistrationCommand> _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ITelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;

    public RegistrationCommand(
        ILoggerFactory loggerFactory, 
        IMapper mapper, 
        IUserService userService,
        ITelegramBotClient botClient, 
        IKeyboardService keyboardService)
    {
        _logger = loggerFactory.CreateLogger<RegistrationCommand>();
        _mapper = mapper;
        _userService = userService;
        _botClient = botClient;
        _keyboardService = keyboardService;
    }

    public CommandTypeEnum CommandType => CommandTypeEnum.Registration;
    
    public CommandTypeEnum BackCommandType => CommandTypeEnum.Empty;

    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        var user = _mapper.Map<Contact, User>(message.Contact); 
        var isCreate = await _userService.CreateIfNotExistAsync(user, cancellationToken);
        
        if (!isCreate)
        {
            _logger.LogInformation($"User { user.TelegramId } already exists");
            await _botClient.SendMessage(message.Chat.Id, MessageText.UserIsAlreadyRegistered, 
                replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
        }
        else
        {
            _logger.LogInformation($"User { user.TelegramId} successfully registered");
            await _botClient.SendMessage(message.Chat.Id, MessageText.YouHaveSuccessfullyRegistered, 
                replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);  
        }

        await _botClient.SendMessage(message.Chat.Id, MessageText.Rules, replyMarkup:
            _keyboardService.GetMainMenu(), cancellationToken: cancellationToken);
    }
}