using Shared.Messages.Repository.Repository.Entitys;

namespace DogLive.TelegramBot.Data.Context.Entity
{
    /// <summary>
    /// Представляет собой сущность, содержащую информацию о
    /// запросах вызова пользователей в приложении.
    /// </summary>
    public class UserCallbackQuery : BaseEntity<int>
    {
        /// <summary>
        /// Уникальный идентификатор запроса обратного вызова.
        /// </summary>
        public string CallbackQueryId { get; set; }
    
        /// <summary>
        /// Идентификатор пользователя в Telegram.
        /// </summary>
        public long UserTelegramId { get; set; }
    
        /// <summary>
        /// Дополнительные данные.
        /// </summary>
        public string? Data { get; set; }
    
        /// <summary>
        /// Идентификатор чата.
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public User User { get; set; }
    }
}