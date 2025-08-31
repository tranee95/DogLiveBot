using DogLive.TelegramBot.Data.Context.Entity;
using DogLive.TelegramBot.Data.Models;
using DogLive.TelegramBot.Data.Models.Booking;
using DogLive.TelegramBot.Data.Models.CommandData;

namespace DogLive.TelegramBot.BL.Services.ServiceInterface.Booking;
/// <summary>
/// Интерфейс для сервиса управления бронированием.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Получает список активных дней для бронирования.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для операции.</param>
    /// <returns>Список активных дней в виде коллекции <see cref="DaysDto"/>.</returns>
    Task<ICollection<DaysDto>> GetActiveDays(CancellationToken cancellationToken);
    
    /// <summary>
    /// Получает доступные временные слоты для указанного дня недели.
    /// </summary>
    /// <param name="day">День недели, для которого нужно получить доступные временные слоты.</param>
    /// <param name="cancellationToken">Токен отмены для операции.</param>
    /// <returns>Список доступных временных слотов в виде коллекции <see cref="AvailableTimeDto"/>.</returns>
    Task<ICollection<AvailableTimeDto>> GetAvailableTimes(DayOfWeek day, CancellationToken cancellationToken);
    
    /// <summary>
    /// Получает временной слот по его идентификатору.
    /// </summary>
    /// <param name="timeSlotId">Идентификатор временного слота.</param>
    /// <param name="cancellationToken">Токен отмены для операции.</param>
    /// <returns>Объект <see cref="AvailableSlot"/> или null, если слот не найден.</returns>
    Task<AvailableSlot?> GetTimeSlot(int timeSlotId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Проверяет, нужно ли пользователю выбирать собаку для бронирования.
    /// </summary>
    /// <param name="telegramUserId">Идентификатор пользователя Telegram.</param>
    /// <param name="cancellationToken">Токен отмены для операции.</param>
    /// <returns>Кортеж, содержащий флаг необходимости выбора собаки и список собак в виде коллекции <see cref="DogDto"/>.</returns>
    Task<(bool IsNeedSelectDog, ICollection<DogDto> Dogs)> CheckIfNeedSelectDog(long telegramUserId,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Пытается зарезервировать временной слот для указанного пользователя.
    /// </summary>
    /// <param name="telegramUserId">Идентификатор пользователя Telegram.</param>
    /// <param name="payload">Данные о бронировании в виде <see cref="BookingPayload"/>.</param>
    /// <param name="cancellationToken">Токен отмены для операции.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task TryReserveSlot(long telegramUserId, BookingPayload payload, CancellationToken cancellationToken);
    
    /// <summary>
    /// Определяет следующий шаг в процессе бронирования на основе переданных данных.
    /// </summary>
    /// <param name="payload">Данные о бронировании в виде <see cref="BookingPayload"/>.</param>
    /// <param name="telegramUserId">Идентификатор пользователя Telegram.</param>
    /// <param name="cancellationToken">Токен отмены для операции.</param>
    /// <returns>Результат следующего шага в процессе бронирования в виде <see cref="BookingStepResult"/>.</returns>
    Task<BookingStepResult> DetermineNextStep(BookingPayload payload, long telegramUserId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получает заметки пользователя по его идентификатору чата.
    /// </summary>
    /// <param name="chatId">Идентификатор чата пользователя.</param>
    /// <param name="cancellationToken">Токен отмены для операции.</param>
    /// <returns>Список заметок пользователя в виде коллекции <see cref="BookingNotesDto"/>.</returns>
    Task<ICollection<BookingNotesDto>> GeUserNotes(long chatId, CancellationToken cancellationToken);
}