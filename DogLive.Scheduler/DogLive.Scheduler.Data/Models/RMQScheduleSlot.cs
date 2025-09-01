using Shared.Messages.Messages.Schedule.Model;

namespace DogLive.Scheduler.Data.Models;

public class RMQScheduleSlot : IRMQScheduleSlot
{
    public RMQScheduleSlot(ICollection<ScheduleSlot> slots)
    {
        ScheduleSlot = slots;
    }

    public ICollection<ScheduleSlot> ScheduleSlot { get; set; }
}