using System.ComponentModel;
using System.Reflection;
using DogLiveBot.BL.Command.CommandFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLiveBot.Data.Enums;
using DogLiveBot.Data.Enums.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Handlers.Messages.MessageHandlerImplementation;

public class TextMessageHandler : IMessageHandler
{
    private readonly ILogger<TextMessageHandler> _logger;
    private readonly ICommandFactory _commandFactory;

    public TextMessageHandler(
        ILogger<TextMessageHandler> logger,
        ICommandFactory commandFactory)
    {
        _commandFactory = commandFactory;
        _logger = logger;
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
            _logger.LogInformation($"Received {(callbackQuery == null ? "message" : "callbackQuery" )}: {text}");
            var commandType = CommandTypeEnumHelper.GetCommandTypeEnum(text);

            if (commandType.HasValue)
            {
                var command = _commandFactory.GetCommand(commandType.Value);
                await command.Execute(executeMessage, cancellationToken, callbackQuery);
            }
        }
    }
}