namespace DogLive.Scheduler.BL.Services.ServiceInterface.Schedule;

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
    Task CreateAndSendAvalableSlot(CancellationToken cancellationToken);
}