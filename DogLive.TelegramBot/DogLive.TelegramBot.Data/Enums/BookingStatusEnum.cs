using System.ComponentModel;

namespace DogLive.TelegramBot.Data.Enums;

/// <summary>
/// Предоставляет перечень возможных статусов бронирования с описанием.
/// </summary>
public enum BookingStatusEnum
{
    [Description("Ожидает")] 
    Awaiting = 1,

    [Description("Подтверждено")] 
    Сonfirmed = 2,

    [Description("Отменено")] 
    Сancelled = 3
}