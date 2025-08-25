using System.Text.Json.Serialization;

namespace DogLiveBot.Data.Models.CommadData;

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
    /// День недели
    /// </summary>
    [JsonPropertyName("dw")]
    public int DayOfWeek { get; set; }

    /// <summary>
    /// Идентификатор слота времени
    /// </summary>
    [JsonPropertyName("ts")]
    public int TimeSlotId { get; set; }

    /// <summary>
    /// идентификатор собаки
    /// </summary>
    [JsonPropertyName("d")]
    public int DogId { get; set; }
}