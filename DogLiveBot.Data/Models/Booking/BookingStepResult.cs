using DogLiveBot.Data.Enums;

namespace DogLiveBot.Data.Models.Booking;

public class BookingStepResult
{
    public BookingStepResult()
    {
    }

    public BookingStepResult(BookingStepEnum step, object? data = null, string? errorMessage = null)
    {
        Step = step;
        Data = data;
        ErrorMessage = errorMessage;
    } 
    
    /// <summary>
    /// Шаг бронирования
    /// </summary>
    public BookingStepEnum Step { get; set; }
    
    // Данные
    public object? Data { get; set; }
    
    // Ошибка
    public string? ErrorMessage { get; set; }
}