using AutoMapper;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Extensions;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLiveBot.BL.Commands.CommandImplementation;

public abstract class CallbackQueryCommand : ICommand
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CallbackQuery, UserCallbackQuery>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => default(int)))
                .ForMember(dest => dest.UserTelegramId, opt => opt.MapFrom(src => src.From.Id))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Message.Chat.Id))
                .ForMember(dest => dest.CallbackQueryId, opt => opt.MapFrom(src => src.Id));
        }
    }

    private readonly IMapper _mapper;
    private readonly ITelegramBotClient _botClient;
    private readonly IRepository _repository;
    private readonly IReadOnlyRepository _readOnlyRepository;

    protected CallbackQueryCommand(
        IMapper mapper,
        ITelegramBotClient telegramBotClient,
        IRepository repository, 
        IReadOnlyRepository readOnlyRepository)
    {
        _mapper = mapper;
        _botClient = telegramBotClient;
        _repository = repository;
        _readOnlyRepository = readOnlyRepository;
    }

    /// <summary>
    /// Получает тип команды
    /// </summary>
    public abstract CommandTypeEnum CommandType { get; }

    /// <summary>
    /// Тип команды для возврата
    /// </summary>
    public abstract CommandTypeEnum BackCommandType { get; }

    /// <summary>
    /// Выполняет обработку сообщения и обратного вызова.
    /// Обрабатывает обратный вызов, обновляет информацию о последнем запросе пользователя
    /// и выполняет специфичную для команды логику.
    /// </summary>
    /// <param name="message">Сообщение, полученное от Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="callbackQuery">Обратный вызов, полученный от Telegram.</param>
    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await ExecuteCommandLogic(message, cancellationToken, callbackQuery);
        await HandleCallbackQuery(callbackQuery, cancellationToken);
    }

    /// <summary>
    /// Выполняет специфичную для команды логику обработки сообщения.
    /// Этот метод должен быть реализован в производных классах.
    /// </summary>
    /// <param name="message">Сообщение, полученное от Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="callbackQuery">Обратный вызов, полученный от Telegram.</param>
    protected abstract Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery);

    /// <summary>
    /// Обрабатывает обратный вызов, обновляет информацию о последнем запросе пользователя
    /// и отвечает на обратный вызов в Telegram.
    /// </summary>
    /// <param name="callbackQuery">Обратный вызов, полученный от Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    private async Task HandleCallbackQuery(CallbackQuery? callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery != null)
        {
            await CreateOrUpdateUserCallbackQuery(callbackQuery, cancellationToken);
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Обновляет информацию о последнем запросе пользователя или добавляет новый,
    /// если его ещё нет в базе данных.
    /// </summary>
    /// <param name="callbackQuery">Обратный вызов, полученный от Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    private async Task CreateOrUpdateUserCallbackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var userLastCallbackQuery = await _readOnlyRepository.GetLast<UserCallbackQuery>(
            filter: s => s.UserTelegramId == callbackQuery.From.Id,
            cancellationToken: cancellationToken);

        if (userLastCallbackQuery == null)
        {
            var userCallbackQuery = _mapper.Map<CallbackQuery, UserCallbackQuery>(callbackQuery);
            await _repository.Add(userCallbackQuery, cancellationToken);
        }
        else
        {
            userLastCallbackQuery.CallbackQueryId = callbackQuery.Id;
            userLastCallbackQuery.Data = CommandType.GetDescription();
            await _repository.Update(userLastCallbackQuery, cancellationToken);
        }
    }
}