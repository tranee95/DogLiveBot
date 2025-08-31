using Shared.Messages.Repository.Repository.Entitys;

namespace DogLive.TelegramBot.Data.Context.Entity
{
    /// <summary>
    /// Представляет сущность собаки, связанной с пользователем.
    /// </summary>
    public class Dog : BaseEntity<int>
    {
        public Dog()
        {
        }

        public Dog(string text, long Id)
        {
            Name = text.Trim();
            UserTelegramId = Id;
        }

        /// <summary>
        /// Идентификатор пользователя в Telegram.
        /// </summary>
        public long UserTelegramId { get; set; }

        /// <summary>
        /// Хозяин собаки
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Получает или устанавливает имя собаки.
        /// </summary>
        public string Name { get; set; }
    }
}