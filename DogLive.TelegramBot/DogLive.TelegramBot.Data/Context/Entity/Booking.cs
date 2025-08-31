using DogLive.TelegramBot.Data.Enums;
using Shared.Messages.Repository.Repository.Entitys;

namespace DogLive.TelegramBot.Data.Context.Entity
{
    /// <summary>
    /// Представляет сущность бронирования, связанного с событием.
    /// Содержит информацию о событии, пользователе, времени бронирования 
    /// и текущем статусе бронирования.
    /// </summary>
    public class Booking : BaseEntity<int>
    {
        public Booking()
        {
        }

        public Booking(long telegramUserId, int dogId, int availableSlotId)
        {
            TelegramUserId = telegramUserId;
            DogId = dogId;
            AvailableSlotId = availableSlotId;

            BookedAt = DateTime.UtcNow;
            Status = BookingStatusEnum.Awaiting;
        }

        /// <summary>
        /// Идентификатор пользователя, осуществившего бронирование.
        /// </summary>
        public long TelegramUserId { get; set; }

        /// <summary>
        /// Идентификатор собаки.
        /// </summary>
        public int DogId { get; set; }

        /// <summary>
        /// Идентификатор события, к которому относится бронирование.
        /// </summary>
        public int AvailableSlotId { get; set; }

        /// <summary>
        /// Время, когда было создано бронирование.
        /// </summary>
        public DateTime BookedAt { get; set; }

        /// <summary>
        /// Текущий статус бронирования.
        /// </summary>
        public BookingStatusEnum Status { get; set; }

        /// <summary>
        /// Пользователь, осуществивший бронирование.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Собака пользователя
        /// </summary>
        public Dog Dog { get; set; }

        /// <summary>
        /// Слот времени
        /// </summary>
        public AvailableSlot AvailableSlot { get; set; }
    }
}