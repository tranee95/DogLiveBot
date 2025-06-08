using Quartz;

namespace DogLiveBot.Data.Models.Quartz;

/// <summary>
/// Задача
/// </summary>
public class JobConfiguration
{
    public JobConfiguration(string name, string cronExpression)
    {
        Key = new JobKey(name);
        JobKeyName = Key.Name;
        TriggerName = $"{JobKeyName}-trigger";
        CronExpression = cronExpression;
    }

    /// <summary>
    /// Ключ задачи.
    /// </summary>
    public JobKey Key { get; private set; }

    /// <summary>
    /// Наименования ключа задачи.
    /// </summary>
    public string JobKeyName { get; private set; }

    /// <summary>
    /// Наименование тригера.
    /// </summary>
    public string TriggerName { get; private set; }

    /// <summary>
    /// Крон выражение.
    /// </summary>
    public string CronExpression { get; private set; }
}