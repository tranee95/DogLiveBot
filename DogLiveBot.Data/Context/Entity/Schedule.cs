namespace DogLiveBot.Data.Context.Entity
{
    /// <summary>
    /// Представляет сущность расписания, определяющую недельное расписание.
    /// Включает даты начала и окончания недели, а также коллекцию
    /// доступных временных интервалов в рамках этой недели.
    /// </summary>
    public class Schedule : BaseEntity<Guid>
    {
        public Schedule()
        {
        }
        
        public Schedule(DateTime weekStartDate, DateTime weekEndDate, bool isActiveWeek)
        {
            WeekStartDate = weekStartDate;
            WeekEndDate = weekEndDate;
            IsActiveWeek = isActiveWeek;
        }

        /// <summary>
        /// Дата начала недели.
        /// </summary>
        public DateTime WeekStartDate { get; set; }

        /// <summary>
        /// Дата окончания недели.
        /// </summary>
        public DateTime WeekEndDate { get; set; }

        /// <summary>
        /// Является ли неделя активной
        /// </summary>
        public bool IsActiveWeek { get; set; }

        /// <summary>
        /// Коллекция доступных временных интервалов,
        /// представляющая периоды недели, в которые возможны
        /// определённые действия или бронирования.
        /// </summary>
        public List<AvailableSlot> AvailableSlots { get; set; } = new();
    }
}