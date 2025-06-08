using System.ComponentModel;

namespace DogLiveBot.Data.Enums;

/// <summary>
/// Предоставляет перечень возможных статусов бронирования с описанием.
/// </summary>
public enum BookingStatusEnum
{
   [Description("Ожидает")]
   Awaiting,
   
   [Description("Подтверждено")]
   Сonfirmed,
   
   [Description("Отменено")]
   Сancelled
}