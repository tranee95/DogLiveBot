using System.ComponentModel;
using System.Reflection;
using DogLiveBot.BL.Command.CommandFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLiveBot.Data.Enums;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Handlers.Messages.MessageHandlerImplementation;

public class TextMessageHandler : IMessageHandler
{
    private readonly ICommandFactory _commandFactory;
    private readonly ILogger<TextMessageHandler> _logger;

    public TextMessageHandler(
        ICommandFactory commandFactory,
        ILogger<TextMessageHandler> logger)
    {
        _commandFactory = commandFactory;
        _logger = logger;
    }

    public MessageType MessageType => MessageType.Text;

    /// <summary>
    /// Обрабатывает обновления, полученные от Telegram API.
    /// </summary>
    /// <param name="message">Сообщение, полученное от Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task HandleMessage(Message message, CancellationToken cancellationToken)
    {
        if (message?.Text != null)
        {
            _logger.LogInformation($"Received message: {message.Text}");
            var commandType = GetCommandTypeEnum(message.Text);
            if (commandType.HasValue)
            {
                var command = _commandFactory.GetCommand(commandType.Value);
                await command.Execute(message, cancellationToken);
            }
        }
        else
        {
            _logger.LogWarning("Received an empty message or a message without text.");
        }
    }

    /// <summary>
    /// Получает значение перечисления <see cref="CommandTypeEnum"/> по заданной строке описания команды.
    /// </summary>
    /// <param name="commandTextType">Строка, представляющая описание команды, для которой необходимо получить значение перечисления.</param>
    /// <returns>
    /// Возвращает значение <see cref="CommandTypeEnum"/>, соответствующее заданному описанию команды,
    /// или <c>null</c>, если описание не найдено.
    /// </returns>
    private static CommandTypeEnum? GetCommandTypeEnum(string commandTextType)
    {
        var type = typeof(CommandTypeEnum);
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null &&
                attribute.Description.Equals(commandTextType, StringComparison.OrdinalIgnoreCase))
            {
                return (CommandTypeEnum)field.GetValue(null);
            }
        }

        return null;
    }
}