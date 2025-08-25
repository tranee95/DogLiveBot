namespace DogLiveBot.Data.Context.Entity
{
    /// <summary>
    /// Представляет сущность доступного временного интервала, связанного с расписанием.
    /// Содержит информацию о дне недели, времени начала и окончания, а также 
    /// доступности интервала.
    /// </summary>
    public class AvailableSlot : BaseEntity<int>
    {
        public AvailableSlot()
        {
        }

        public AvailableSlot(int scheduleId, DayOfWeek dayOfWeek, TimeSpan time, TimeSpan interval, 
            bool isReserved = false)
        {
            ScheduleId = scheduleId;
            DayOfWeek = dayOfWeek;
            Date = DateTime.UtcNow.AddDays((int)dayOfWeek);
            StartTime = time;
            EndTime = time.Add(interval);
            IsReserved = isReserved;
        }

        /// <summary>
        /// Идентификатор связанного расписания.
        /// </summary>
        public int ScheduleId { get; set; }

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

        /// <summary>
        /// Связанное расписание, к которому относится данный интервал.
        /// </summary>
        public Schedule Schedule { get; set; }
    }
}