using DogLive.Scheduler.BL.Services.ServiceInterface.Schedule;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DogLive.Scheduler.BL.Jobs;

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
                await _scheduleService.CreateAndSendAvalableSlot(cts.Token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in filling calendar data job");
        }
    }
}