namespace DogLiveBot.BL.Services.ServiceInterface;

/// <summary>
/// Интерфейс для сервиса управления расписанием.
/// </summary>
public interface IScheduleService
{
    /// <summary>
    /// Заполняет календарь.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    public Task FillCalendar(CancellationToken cancellationToken);
}