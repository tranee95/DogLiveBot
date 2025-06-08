using DogLiveBot.Data.Enums;

namespace DogLiveBot.Data.Context.Entity
{
    /// <summary>
    /// Представляет сущность бронирования, связанного с событием.
    /// Содержит информацию о событии, пользователе, времени бронирования 
    /// и текущем статусе бронирования.
    /// </summary>
    public class Booking : BaseEntity<Guid>
    {
        /// <summary>
        /// Идентификатор связанного события.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, осуществившего бронирование.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Время, когда было создано бронирование.
        /// </summary>
        public DateTime BookedAt { get; set; }

        /// <summary>
        /// Текущий статус бронирования.
        /// </summary>
        public BookingStatusEnum Status { get; set; }

        /// <summary>
        /// Связанное событие, к которому относится бронирование.
        /// </summary>
        public Event Event { get; set; }

        /// <summary>
        /// Пользователь, осуществивший бронирование.
        /// </summary>
        public User User { get; set; }
    }
}