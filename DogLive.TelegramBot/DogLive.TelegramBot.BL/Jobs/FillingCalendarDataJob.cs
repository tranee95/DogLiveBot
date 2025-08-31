using DogLive.TelegramBot.BL.Services.ServiceInterface.Schedule;
using DogLive.TelegramBot.BL.Services.ServiceInterface;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DogLive.TelegramBot.BL.Jobs;

/// <summary>
/// Задача для заполнения данных календаря с использованием Quartz.
/// </summary>
public class FillingCalendarDataJob : IJob
{
    private readonly ILogger<FillingCalendarDataJob> _logger;
    private readonly IScheduleService _scheduleService;
    
    public FillingCalendarDataJob(
        ILogger<FillingCalendarDataJob> logger, 
        IScheduleService scheduleService)
    {
        _logger = logger;
        _scheduleService = scheduleService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            using (var cts = new CancellationTokenSource())
            {
                _logger.LogInformation("Start filling calendar data job");
                await _scheduleService.FillCalendar(cts.Token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in filling calendar data job");
        }
    }
}