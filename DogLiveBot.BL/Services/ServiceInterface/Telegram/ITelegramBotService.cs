namespace DogLiveBot.BL.Services.ServiceInterface.Telegram;

public interface ITelegramBotService
{
    /// <summary>
    /// Запускает службу Telegram-бота и начинает прослушивание обновлений.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Start(CancellationToken cancellationToken);
}