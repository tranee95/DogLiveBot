namespace DogLive.TelegramBot.Data.Models.Options
{
    /// <summary>
    /// Класс для хранения настроек подключения и параметров работы с Redis.
    /// </summary>
    public class RedisSettings
    {
        /// <summary>
        /// Хост Redis-сервера.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Порт Redis-сервера.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Время жизни кеша в минутах.
        /// </summary>
        public int LiveTimeMinutes { get; set; }
    }
}