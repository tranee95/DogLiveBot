using DogLiveBot.BL.Handlers.Messages.MessageHandlerFactory;
using DogLiveBot.BL.Services.ServiceInterface;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DogLiveBot.BL.Services.ServiceImplementation
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IMessageHandlerFactory _messageHandlerFactory;

        private readonly ReceiverOptions _receiverOptions;

        public TelegramBotService(
            ITelegramBotClient botClient,
            ILoggerFactory loggerFactory, 
            IMessageHandlerFactory messageHandlerFactory)
        {
            _logger = loggerFactory.CreateLogger<TelegramBotService>();
            _botClient = botClient;
            _messageHandlerFactory = messageHandlerFactory;

            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// Запускает службу Telegram-бота и начинает прослушивание обновлений.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены, используемый для управления жизненным циклом операции.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        public async Task Start(CancellationToken cancellationToken)
        {
            _botClient.StartReceiving(HandleUpdate, HandleError, _receiverOptions, cancellationToken: cancellationToken);
            var me = await _botClient.GetMe(cancellationToken);

            _logger.LogInformation($"Start listening for @{me.Username}");
        }

        /// <summary>
        /// Обрабатывает обновления, полученные от Telegram API.
        /// </summary>
        /// <param name="botClient">Клиент Telegram Bot.</param>
        /// <param name="update">Обновление, полученное от Telegram.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Message != null)
                {
                    var handler = _messageHandlerFactory.GetMessageHandler(update.Message.Type);
                    await handler.HandleMessage(update.Message, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling update");
            }
        }

        /// <summary>
        /// Обрабатывает ошибки, возникающие при взаимодействии с Telegram API.
        /// </summary>
        /// <param name="botClient">Клиент Telegram Bot.</param>
        /// <param name="exception">Исключение, возникшее во время обработки.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            _logger.LogError(errorMessage);
            return Task.CompletedTask;
        }
    }
}