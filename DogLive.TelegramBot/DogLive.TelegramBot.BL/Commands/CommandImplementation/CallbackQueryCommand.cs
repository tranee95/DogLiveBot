using AutoMapper;
using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.Data.Context.Entity;
using DogLive.TelegramBot.Data.Enums;
using DogLive.TelegramBot.Data.Enums.Extensions;
using DogLive.TelegramBot.Data.Models.CommandData;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DogLive.TelegramBot.BL.Commands.CommandImplementation;

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
    /// Обертка выполнения команды (ICommand): сначала обрабатывает CallbackQuery, затем предметную логику.
    /// </summary>
    public async Task Execute(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await HandleCallbackQuery(callbackQuery, cancellationToken);
        await ExecuteCommandLogic(message, cancellationToken, callbackQuery);
    }

    /// <summary>
    /// Обертка выполнения команды, обрабатывающей текст (IReceivedTextCommand).
    /// Вызывается из конкретной команды в реализации интерфейса.
    /// </summary>
    protected async Task ExecuteTextCommand(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await HandleCallbackQuery(callbackQuery, cancellationToken);
        await ExecuteTextCommandLogicCore(message, cancellationToken, callbackQuery);
    }

    /// <summary>
    /// Обертка выполнения команды, обрабатывающей данные (IReceivedDataCommand).
    /// Вызывается из конкретной команды в реализации интерфейса.
    /// </summary>
    protected async Task ExecuteDataCommand(Message message, CommandData commandData, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        await HandleCallbackQuery(callbackQuery, cancellationToken);
        await ExecuteDataCommandLogicCore(message, commandData, cancellationToken, callbackQuery);
    }

    /// <summary>
    /// Предметная логика для ICommand.
    /// </summary>
    protected abstract Task ExecuteCommandLogic(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery);

    /// <summary>
    /// Предметная логика для IReceivedTextCommand (переопределяется в командах, которые его поддерживают).
    /// </summary>
    protected virtual Task ExecuteTextCommandLogicCore(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        throw new NotSupportedException($"{GetType().Name} does not support text handling");
    }

    /// <summary>
    /// Предметная логика для IReceivedDataCommand (переопределяется в командах, которые его поддерживают).
    /// </summary>
    protected virtual Task ExecuteDataCommandLogicCore(Message message, CommandData commandData,
        CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        throw new NotSupportedException($"{GetType().Name} does not support data handling");
    }

    /// <summary>
    /// Обрабатывает обратный вызов, обновляет информацию о последнем запросе пользователя
    /// и отвечает на обратный вызов в Telegram.
    /// </summary>
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