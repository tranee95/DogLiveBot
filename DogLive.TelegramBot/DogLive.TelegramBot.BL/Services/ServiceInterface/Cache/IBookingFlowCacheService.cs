using DogLive.TelegramBot.Data.Models.Cache;

namespace DogLive.TelegramBot.BL.Services.ServiceInterface.Cache;

public interface IBookingFlowCacheService
{
    /// <summary>
    /// Асинхронно получает состояние потока бронирования для указанного пользователя Telegram.
    /// </summary>
    /// <param name="telegramUserId">Идентификатор пользователя Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Объект <see cref="BookingFlowCache"/>, представляющий состояние потока бронирования.</returns>
    Task<BookingFlowCache> GetAsync(long telegramUserId, CancellationToken cancellationToken);

    /// <summary>
    /// Асинхронно устанавливает состояние потока бронирования для указанного пользователя Telegram с заданным временем жизни.
    /// </summary>
    /// <param name="telegramUserId">Идентификатор пользователя Telegram.</param>
    /// <param name="state">Состояние потока бронирования для установки.</param>
    /// <param name="ttl">Время жизни состояния (Time-to-Live).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task SetAsync(long telegramUserId, BookingFlowCache state, TimeSpan ttl, CancellationToken cancellationToken);

    /// <summary>
    /// Асинхронно очищает состояние потока бронирования для указанного пользователя Telegram.
    /// </summary>
    /// <param name="telegramUserId">Идентификатор пользователя Telegram.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task ClearAsync(long telegramUserId, CancellationToken cancellationToken);
}