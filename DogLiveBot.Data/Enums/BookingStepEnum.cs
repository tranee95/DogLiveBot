namespace DogLiveBot.Data.Enums;

public enum BookingStepEnum
{
    // Выбор дня
    SelectDay = 1,
    
    // Выбор времени
    SelectTime = 2,
    
    // Выбор собаки
    SelectDog = 3,
    
    // Резерв
    Reserve = 4,
    
    // Перезапуск
    Restart = 5
}