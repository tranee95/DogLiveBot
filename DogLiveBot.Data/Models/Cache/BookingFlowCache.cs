namespace DogLiveBot.Data.Models.Cache;

public sealed class BookingFlowCache
{
    public DayOfWeek? DayOfWeek { get; set; }
    public TimeSpan? Time { get; set; }
    public DateOnly? Date { get; set; }
}

public static class BookingCacheKeys
{
    public static string ForUser(long telegramUserId) => $"booking:flow:{telegramUserId}";
}
