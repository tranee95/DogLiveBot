using System.Globalization;

namespace DogLiveBot.Data.Models.Booking;

public class BookingNotesDto
{
    public BookingNotesDto()
    {
    }

    public BookingNotesDto(int id, 
        DayOfWeek dayOfWeek, 
        TimeSpan startTime, 
        TimeSpan endTime,
        DateTime data)
    {
        Id = id;

        _dayOfWeek = dayOfWeek;
        _startTime = startTime;
        _endTime = endTime;
        _date = data;
    }

    private readonly CultureInfo Ru = new("ru-RU");

    private DayOfWeek _dayOfWeek { get; }
    private TimeSpan _startTime { get; }
    private TimeSpan _endTime { get; }
    private DateTime _date { get; }

    public int Id { get; }
    public string DayOfWeekName => Ru.DateTimeFormat.GetDayName(_dayOfWeek);
    public string StartAndEndTime => $@"{_startTime:hh\:mm} - {_endTime:hh\:mm}";
    public string Date => _date.ToString("dd.MM.yyyy");
    public string Text => $"{Date} {StartAndEndTime} ({DayOfWeekName})";
    public int Hour => _startTime.Hours;
}