using DogLiveBot.Data.Enums;

namespace DogLiveBot.Data.Models;

public class DaysDto
{
    /// <summary>
    /// Текстовая дата дня недели для кнопки
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// День недели
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Команда
    /// </summary>
    public CommandTypeEnum CommandType => CommandTypeEnum.CreateBooking;  
}