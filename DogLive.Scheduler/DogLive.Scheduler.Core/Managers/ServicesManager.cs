using DogLive.Scheduler.BL.Jobs;
using DogLive.Scheduler.BL.Services.ServiceImplementation.Schedule;
using DogLive.Scheduler.BL.Services.ServiceInterface.Schedule;
using DogLive.Scheduler.Core.Managers.Extensions;
using DogLive.Scheduler.Data.Models.Options;
using DogLive.Scheduler.Data.Models.Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using Shared.Messages.Messages.Extensions;

namespace DogLive.Scheduler.Core.Managers;

public static class ServicesManager
{
    /// <summary>
    /// Конфигурирует сервисы для приложения.
    /// </summary>
    /// <param name="builder">Строитель хоста, используемый для конфигурации сервисов.</param>
    public static void ConfigureServices(IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            RegisterOptions(services, context);
            RegisterServices(services);
            RegisterJobs(services);
            RegisterMassTransit(services);
        });
    }

    /// <summary>
    /// Регистрирует параметры приложения в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="context">Контекст хоста, содержащий конфигурацию приложения.</param>
    private static void RegisterOptions(IServiceCollection services, HostBuilderContext context)
    {
        services.AddOptions<ApplicationOptions>()
            .Bind(context.Configuration.GetSection(nameof(ApplicationOptions)))
            .ValidateDataAnnotations();
    }
    
    /// <summary>
    /// Метод для регистрации заданий (Jobs) с использованием Quartz в контейнере службы.
    /// Извлекает настройки приложения из конфигурации, включая крон-выражения,
    /// и добавляет задание <see cref="FillingCalendarDataJob"/> с заданной конфигурацией.
    /// </summary>
    /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
    private static void RegisterJobs(IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
        services.AddQuartz(q =>
        {
            AddJob<FillingCalendarDataJob>(q, new JobConfiguration(nameof(settings.CronExpressionSettings), 
                settings.CronExpressionSettings.StartFillingCalendarData));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }

    /// <summary>
    /// Добавление задачи на исполнение
    /// </summary>
    /// <param name="configurator">Конфигуратор</param>
    /// <param name="jobConfiguration">Конфигурация задачи</param>
    /// <typeparam name="T">Задача</typeparam>
    private static void AddJob<T>(IServiceCollectionQuartzConfigurator configurator,
        JobConfiguration jobConfiguration) where T : IJob
    {
        configurator.AddJob<T>(options => options.WithIdentity(jobConfiguration.JobKeyName));
        configurator.AddTrigger(options => options
            .ForJob(jobConfiguration.Key)
            .WithIdentity(jobConfiguration.TriggerName)
            .WithCronSchedule(jobConfiguration.CronExpression)
        );
    }

    /// <summary>
    /// Регистрирует дополнительные сервисы в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    private static void RegisterServices(IServiceCollection services)
    {
        services.AddAutoMapperProfiles();
        services.AddScoped<IScheduleService, ScheduleService>();
    }

    /// <summary>
    /// Регистрация massTransit.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    private static void RegisterMassTransit(IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
        services.RegisterMassTransit(settings.RabbitMqSettings.ConnectionString);
    }
}