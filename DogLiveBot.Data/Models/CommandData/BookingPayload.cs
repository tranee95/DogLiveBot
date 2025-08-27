using System.Text.Json.Serialization;

namespace DogLiveBot.Data.Models.CommandData;

/// <summary>
/// Представляет данные о бронировании, включая день недели, идентификатор временного слота и идентификатор собаки.
/// </summary>
public class BookingPayload
{
    public BookingPayload()
    {
    }

    public BookingPayload(DayOfWeek dayOfWeek, int? timeSlotId, int? dogId)
    {
        DayOfWeek = (int)dayOfWeek;
        if (timeSlotId != null)
        {
            TimeSlotId = timeSlotId.Value;
        }

        if (dogId != null)
        {
            DogId = dogId.Value;
        }
    }

    /// <summary>
    /// День недели.
    /// </summary>
    [JsonPropertyName("dw")]
    public int DayOfWeek { get; set; }

    /// <summary>
    /// Идентификатор слота времени.
    /// </summary>
    [JsonPropertyName("ts")]
    public int TimeSlotId { get; set; }

    /// <summary>
    /// Идентификатор собаки.
    /// </summary>
    [JsonPropertyName("d")]
    public int DogId { get; set; }

    /// <summary>
    /// Проверяет, все ли свойства имеют значения по умолчанию.
    /// </summary>
    /// <returns>Истина, если все свойства имеют значения по умолчанию; иначе — ложь.</returns>
    public bool IsAllPropertyDefault()
    {
        return DayOfWeek == default && TimeSlotId == default && DogId == default;
    }

    /// <summary>
    /// Проверяет, имеет ли день недели значение по умолчанию.
    /// </summary>
    /// <returns>Истина, если день недели имеет значение по умолчанию; иначе — ложь.</returns>
    public bool IsDayOfWeekDefault()
    {
        return DayOfWeek == default;
    }

    /// <summary>
    /// Проверяет, имеет ли идентификатор слота времени значение по умолчанию.
    /// </summary>
    /// <returns>Истина, если идентификатор слота времени имеет значение по умолчанию; иначе — ложь.</returns>
    public bool IsTimeSlotIdDefault()
    {
        return TimeSlotId == default;
    }

    /// <summary>
    /// Проверяет, имеет ли идентификатор собаки значение по умолчанию.
    /// </summary>
    /// <returns>Истина, если идентификатор собаки имеет значение по умолчанию; иначе — ложь.</returns>
    public bool IsDogIdDefault()
    {
        return DogId == default;
    }

    /// <summary>
    /// Проверяет, все ли свойства имеют значения, отличные от значений по умолчанию.
    /// </summary>
    /// <returns>Истина, если все свойства не равны значениям по умолчанию; иначе — ложь.</returns>
    public bool IsAllPropertyNotDefault()
    {
        return DayOfWeek != default && TimeSlotId != default && DogId != default;
    }
}