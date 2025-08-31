namespace DogLive.TelegramBot.Data.Models;

public class AvailableTimeDto
{
    /// <summary>
    /// Id Слота
    /// </summary>
    public int TimeSlotId { get; set; }
    
    /// <summary>
    /// Начало занятия
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Окончания занятия
    /// </summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Заголовок
    /// </summary>
    public string Label => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
}