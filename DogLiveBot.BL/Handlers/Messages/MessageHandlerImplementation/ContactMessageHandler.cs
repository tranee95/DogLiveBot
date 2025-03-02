using DogLiveBot.BL.Command.CommandFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLiveBot.Data.Enums;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Handlers.Messages.MessageHandlerImplementation;

public class ContactMessageHandler : IMessageHandler
{
    private readonly ICommandFactory _commandFactory;
    private readonly ILogger<ContactMessageHandler> _logger;

    public ContactMessageHandler(
        ICommandFactory commandFactory,
        ILogger<ContactMessageHandler> logger)
    {
        _commandFactory = commandFactory;
        _logger = logger;
    }

    public MessageType MessageType => MessageType.Contact;

    /// <summary>
    /// Обрабатывает входящие сообщения от пользователей.
    /// </summary>
    /// <param name="message">Сообщение, полученное от пользователя.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task HandleMessage(Message message, CancellationToken cancellationToken, CallbackQuery? callbackQuery = null)
    {
        if (message != null)
        {
            _logger.LogInformation($"Received contact: {message.Text}");
            var command = _commandFactory.GetCommand(CommandTypeEnum.Registration);
            await command.Execute(message, cancellationToken);
        }
    }
}