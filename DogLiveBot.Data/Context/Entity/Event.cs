namespace DogLiveBot.Data.Context.Entity
{
    /// <summary>
    /// Представляет сущность события с информацией о дате, времени и статусе бронирования.
    /// Содержит временные данные о создании и обновлении, а также список связанных бронирований.
    /// </summary>
    public class Event : BaseEntity<Guid>
    {
        /// <summary>
        /// Дата проведения события.
        /// </summary>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Время начала события.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания события.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Указывает, забронировано ли данное событие.
        /// </summary>
        public bool IsBooked { get; set; }

        /// <summary>
        /// Время создания события.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Время последнего обновления события.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Список бронирований, связанных с этим событием.
        /// </summary>
        public List<Booking> Bookings { get; set; } = new();
    }
}