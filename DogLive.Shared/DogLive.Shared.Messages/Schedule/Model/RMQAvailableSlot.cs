using System;
using System.Collections.Generic;

namespace Shared.Messages.Messages.Schedule.Model;

public class RMQAvailableSlot
{
    public RMQAvailableSlot(ICollection<AvailableSlotDto> availableSlots)
    {
        AvailableSlots = availableSlots;
    }

    public ICollection<AvailableSlotDto> AvailableSlots { get; private set; }
}

public class AvailableSlotDto
{
    public AvailableSlotDto()
    {
    }

    public AvailableSlotDto(DayOfWeek dayOfWeek, TimeSpan time, TimeSpan interval,
        bool isReserved = false)
    {
        DayOfWeek = dayOfWeek;
        Date = DateTime.UtcNow.AddDays((int)dayOfWeek);
        StartTime = time;
        EndTime = time.Add(interval);
        IsReserved = isReserved;
    }

    /// <summary>
    /// Дата занятия
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// День недели, к которому относится временной интервал.
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Время начала интервала.
    /// </summary>
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Время окончания интервала.
    /// </summary>
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Определяет, является ли данный временной интервал зарезервированым.
    /// </summary>
    public bool IsReserved { get; set; }
}