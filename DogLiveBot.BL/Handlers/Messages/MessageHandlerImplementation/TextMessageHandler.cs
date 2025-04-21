using DogLiveBot.BL.Command.CommandFactory;
using DogLiveBot.BL.Command.ReceivedTextCommandFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Helpers;
using DogLiveBot.Data.Model;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Handlers.Messages.MessageHandlerImplementation;

public class TextMessageHandler : IMessageHandler
{
    private readonly ILogger<TextMessageHandler> _logger;
    private readonly ICommandFactory _commandFactory;
    private readonly IReceivedTextCommandFactory _receivedTextCommandFactory;
    private readonly IRepository<UserCallbackQuery> _userCallbackQueryRepository;

    public TextMessageHandler(
        ILogger<TextMessageHandler> logger,
        ICommandFactory commandFactory, 
        IReceivedTextCommandFactory receivedTextCommandFactory,
        IRepository<UserCallbackQuery> userCallbackQueryRepository)
    {
        _logger = logger;
        _commandFactory = commandFactory;
        _receivedTextCommandFactory = receivedTextCommandFactory;
        _userCallbackQueryRepository = userCallbackQueryRepository;
    }

    public MessageType MessageType => MessageType.Text;

    /// <summary>
    /// Обрабатывает обновления, полученные от Telegram API.
    /// </summary>
    /// <param name="message">Сообщение, полученное от Telegram.</param>
    /// <param name="callbackQuery">Запрос на обратный вызов.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task HandleMessage(Message message, CancellationToken cancellationToken,
        CallbackQuery? callbackQuery = null)
    {
        var text = callbackQuery?.Data ?? message.Text;
        var executeMessage = callbackQuery?.Message ?? message;

        if (text != null)
        {
            _logger.LogInformation($"Received {(callbackQuery == null ? "message" : "callbackQuery")}: {text}");
            
            if (CommandTypeEnumHelper.TryParseFromJsonToObject<CommandDataModel>(text, out CommandDataModel commandData))
            { 
                //TODO: тут необходимо добавить реализацию обработки данных команды 
                //TODO: HandleReceiveData
                await ExecuteCommand(commandData.CommandType, executeMessage, cancellationToken, callbackQuery);
            }
            else
            {
                var commandType = CommandTypeEnumHelper.GetCommandTypeEnum(text);
                var task = commandType.HasValue 
                    ? ExecuteCommand(commandType.Value, executeMessage, cancellationToken, callbackQuery) 
                    : HandleReceivedText(message, cancellationToken);

                await task.WaitAsync(cancellationToken);
            }
        }
    }

    /// <summary>
    /// Выполняет команду на основе указанного типа команды.
    /// Получает команду из фабрики команд и вызывает её метод Execute.
    /// </summary>
    /// <param name="commandType">Тип команды, которую необходимо выполнить.</param>
    /// <param name="executeMessage">Сообщение, с которым будет работать команда.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="callbackQuery">Обратный вызов, полученный от Telegram, если имеется.</param>
    private async Task ExecuteCommand(CommandTypeEnum commandType, Message executeMessage,
        CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        var command = _commandFactory.GetCommand(commandType);
        await command.Execute(executeMessage, cancellationToken, callbackQuery);
    }

    /// <summary>
    /// Обрабатывает текст, полученный от пользователя, если он не соответствует известной команде.
    /// Получает последний запрос пользователя из репозитория и выполняет соответствующую команду на основе данных этого запроса.
    /// </summary>
    /// <param name="message">Сообщение, полученное от Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    private async Task HandleReceivedText(Message message, CancellationToken cancellationToken)
    {
        var userCallbackQuery = await _userCallbackQueryRepository.Get(
            s => s.UserTelegramId == message.Chat.Id, cancellationToken);
        if (userCallbackQuery != null)
        {
            var commandType = CommandTypeEnumHelper.GetCommandTypeEnum(userCallbackQuery.Data);
            var receivedTextCommand = _receivedTextCommandFactory.GetCommand(commandType);
            await receivedTextCommand.ExecuteReceivedTextLogic(message, cancellationToken);
        }
    }
}