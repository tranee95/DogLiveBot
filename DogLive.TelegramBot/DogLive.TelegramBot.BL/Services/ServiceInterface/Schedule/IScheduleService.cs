using DogLive.TelegramBot.Data.Models;
using DogLive.TelegramBot.Data.Models.CommandData;

namespace DogLive.TelegramBot.BL.Services.ServiceInterface.Schedule;

/// <summary>
/// Интерфейс для сервиса управления расписанием.
/// </summary>
public interface IScheduleService
{
    /// <summary>
    /// Заполняет календарь временными интервалами на текущую неделю.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    Task FillCalendar(CancellationToken cancellationToken);

    /// <summary>
    /// Получает доступные дни недели для записи.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список доступных дней в виде коллекции <see cref="DaysDto"/>.</returns>
    Task<ICollection<DaysDto>> GetActiveDays(CancellationToken cancellationToken);

    /// <summary>
    /// Получает доступные временные интервалы для указанного дня недели.
    /// </summary>
    /// <param name="dayOfWeek">День недели, для которого нужно получить доступное время.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список доступных временных интервалов в виде коллекции <see cref="AvailableTimeDto"/>.</returns>
    Task<ICollection<AvailableTimeDto>> GetAvailableTimes(DayOfWeek dayOfWeek, CancellationToken cancellationToken);

    /// <summary>
    /// Пытается зарезервировать временной интервал для указанного пользователя Telegram.
    /// </summary>
    /// <param name="telegramUserId">Идентификатор пользователя Telegram.</param>
    /// <param name="bookingPayload">Данные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Истина, если резервирование прошло успешно; иначе — ложь.</returns>
    Task<bool> TryReserveSlot(long telegramUserId, BookingPayload bookingPayload,
        CancellationToken cancellationToken);

    /// <summary>
    /// Получение слота для регистрации по идентификатору. 
    /// </summary>
    /// <param name="timeSlotId">Идентификатор слота времени.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Слот времени <see cref="AvailableTimeDto"/>.</returns>
    Task<AvailableTimeDto> GetTimeSlotById(int timeSlotId, CancellationToken cancellationToken);
}