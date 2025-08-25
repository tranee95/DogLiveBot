using System.Text.Json;
using DogLiveBot.BL.Commands.CommandFactory;
using DogLiveBot.BL.Commands.ReceivedDataCommandFactory;
using DogLiveBot.BL.Commands.ReceivedTextCommandFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Helpers;
using DogLiveBot.Data.Models.CommandData;
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
    private readonly IReceivedDataCommandFactory _receivedDataCommandFactory;
    private readonly IReadOnlyRepository _readOnlyRepository;

    public TextMessageHandler(
        ILogger<TextMessageHandler> logger,
        ICommandFactory commandFactory, 
        IReceivedTextCommandFactory receivedTextCommandFactory,
        IReceivedDataCommandFactory receivedDataCommandFactory,
        IReadOnlyRepository readOnlyRepository)
    {
        _logger = logger;
        _commandFactory = commandFactory;
        _receivedTextCommandFactory = receivedTextCommandFactory;
        _receivedDataCommandFactory = receivedDataCommandFactory;
        _readOnlyRepository = readOnlyRepository;
    }

    public MessageType MessageType => MessageType.Text;

    /// <summary>
    /// Обрабатывает обновления, полученные от Telegram API.
    /// Если сообщение или запрос на обратный вызов содержит данные команды, 
    /// выполняет соответствующую обработку. В противном случае обрабатывает текстовое сообщение.
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

            if (TryParseFromJsonToObject<CommandData>(text, out var commandData))
            { 
                await HandleReceivedData(executeMessage, commandData, cancellationToken, callbackQuery);
            }
            else
            {
                var commandType = CommandTypeEnumHelper.GetCommandTypeEnum(text);
                var task = commandType.HasValue 
                    ? HandleExecuteCommand(commandType.Value, executeMessage, cancellationToken, callbackQuery) 
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
    private async Task HandleExecuteCommand(CommandTypeEnum commandType, Message executeMessage,
        CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        var command = _commandFactory.GetCommand(commandType);
        await command.Execute(executeMessage, cancellationToken, callbackQuery);
    }

    /// <summary>
    /// Обрабатывает текст, полученный от пользователя, если он не соответствует известной команде.
    /// Получает последний запрос пользователя из репозитория и выполняет соответствующую команду 
    /// на основе данных этого запроса.
    /// </summary>
    /// <param name="message">Сообщение, полученное от Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    private async Task HandleReceivedText(Message message, CancellationToken cancellationToken)
    {
        var userCallbackQuery = await _readOnlyRepository.GetFirstOrDefault<UserCallbackQuery>(
           filter: s => s.UserTelegramId == message.Chat.Id,
           cancellationToken: cancellationToken);
        
        if (userCallbackQuery != null)
        {
            var commandType = CommandTypeEnumHelper.GetCommandTypeEnum(userCallbackQuery.Data);
            var receivedTextCommand = _receivedTextCommandFactory.GetCommand(commandType);
            await receivedTextCommand.ExecuteReceivedTextLogic(message, cancellationToken);
        }
    }

    /// <summary>
    /// Обрабатывает полученные данные команды.
    /// Получает команду из фабрики команд и вызывает её метод для обработки полученных данных.
    /// </summary>
    /// <param name="executeMessage">Сообщение, с которым будет работать команда.</param>
    /// <param name="commandData">Данные команды, полученные от пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <param name="callbackQuery">Обратный вызов, полученный от Telegram, если имеется.</param>
    private async Task HandleReceivedData(Message executeMessage, CommandData commandData, 
        CancellationToken cancellationToken, CallbackQuery? callbackQuery)
    {
        var receivedDataCommand = _receivedDataCommandFactory.GetCommand(commandData.CommandType);
        await receivedDataCommand.ExecuteReceivedDataLogic(executeMessage, commandData, cancellationToken, callbackQuery);
    }

    /// <summary>
    /// Пытается десериализовать JSON-строку в объект указанного типа.
    /// </summary>
    /// <typeparam name="T">Тип объекта, в который нужно десериализовать JSON.</typeparam>
    /// <param name="json">JSON-строка для десериализации.</param>
    /// <param name="result">Результат десериализации, если она прошла успешно.</param>
    /// <returns>True, если десериализация прошла успешно, иначе False.</returns>
    private static bool TryParseFromJsonToObject<T>(string json, out T result)
    {
        try
        {
            result = JsonSerializer.Deserialize<T>(json);
            return result != null;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}