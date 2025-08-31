namespace DogLive.Scheduler.Data.Models.Options
{
    /// <summary>
    /// Настройки приложения.
    /// </summary>
    public class ApplicationOptions
    {
        /// <summary>
        /// Конфигурация крон выражений.
        /// </summary>
        public CronExpressionSettings CronExpressionSettings { get; set; }  

        /// <summary>
        /// Настройки формирования слотов расписания
        /// </summary>
        public AvailableSlotSettings AvailableSlotSettings { get; set; }
    }
}