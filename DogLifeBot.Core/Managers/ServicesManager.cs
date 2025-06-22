using DogLiveBot.BL.Commands.CommandFactory;
using DogLiveBot.BL.Commands.CommandImplementation;
using DogLiveBot.BL.Commands.CommandInterface;
using DogLiveBot.BL.Commands.ReceivedDataCommandFactory;
using DogLiveBot.BL.Commands.ReceivedTextCommandFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerImplementation;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLiveBot.BL.Jobs;
using DogLiveBot.BL.Services.ServiceImplementation;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Core.Managers.Extensions;
using DogLiveBot.Data.Context;
using DogLiveBot.Data.Models.Options;
using DogLiveBot.Data.Models.Quartz;
using DogLiveBot.Data.Repository.RepositoryImplementations;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Quartz;

namespace DogLiveBot.Core.Managers;

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
            RegisterDbContext(services);
            RegisterTelegramBotClient(services);
            RegisterRedis(services);
            RegisterServices(services);
            RegisterCommands(services);
            RegisterJobs(services);
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
    /// Регистрирует контекст базы данных и репозитории в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    private static void RegisterDbContext(IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
        services.AddDbContextFactory<ApplicationDbContext>(contextOptionsBuilder =>
        {
            contextOptionsBuilder.UseNpgsql(settings.ApplicationDbConnection.ConnectionString);
        });
        services.AddScoped(typeof(IRepository<>), typeof(ApplicationRepository<>));
    }

    /// <summary>
    /// Регистрирует клиент Telegram Bot и сервис Telegram Bot в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    private static void RegisterTelegramBotClient(IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
        services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(settings.TelegramBotSettings.Token));
        services.AddScoped<ITelegramBotService, TelegramBotService>();
    }

    /// <summary>
    /// Регистрирует Redis-кеш в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
    private static void RegisterRedis(IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(opt =>
        {
            var options = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
            opt.Configuration = $"{options.RedisSettings.Host}:{options.RedisSettings.Port}";
        });
    }

    /// <summary>
    /// Метод для регистрации заданий (Jobs) с использованием Quartz в контейнере службы.
    /// Извлекает настройки приложения из конфигурации, включая крон-выражения,
    /// и добавляет задание <see cref="FillingCalendarDataJob"/> с заданной конфигурацией.
    /// </summary>
    /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
    private static void RegisterJobs(IServiceCollection services)
    {
        // Извлечение настроек приложения
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;

        // Регистрация заданий через Quartz
        services.AddQuartz(q =>
        {
            // Добавление задачи на основе настроек крон выражений
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
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IKeyboardService, KeyboardService>();
        services.AddScoped<ICommandService, CommandService>();
        services.AddScoped<IScheduleService, ScheduleService>();

        services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();

        services.AddScoped<IMessageHandler, TextMessageHandler>();
        services.AddScoped<IMessageHandler, ContactMessageHandler>();
        services.AddScoped<ICacheService, CacheService>();
    }

    /// <summary>
    /// Регистрирует команды в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    private static void RegisterCommands(IServiceCollection services)
    {
        services.AddScoped<ICommandFactory, CommandFactory>();

        services.AddScoped<ICommand, StartCommand>();
        services.AddScoped<ICommand, RegistrationCommand>();
        services.AddScoped<ICommand, MyNotesCommand>();
        services.AddScoped<ICommand, RulesCommand>();
        services.AddScoped<ICommand, SettingsCommand>();
        services.AddScoped<ICommand, ShowPaymentDetailsCommand>();
        services.AddScoped<ICommand, SignUpForClassCommand>();
        services.AddScoped<ICommand, RenameCommand>();
        services.AddScoped<ICommand, AddDogCommand>();
        services.AddScoped<ICommand, BackCommand>();
        services.AddScoped<ICommand, MenuCommand>();
        services.AddScoped<ICommand, DeleteDogCommand>();

        services.AddScoped<IReceivedTextCommandFactory, ReceivedTextCommandFactory>();
        services.AddScoped<IReceivedTextCommand, AddDogCommand>();
        services.AddScoped<IReceivedTextCommand, RenameCommand>();

        services.AddScoped<IReceivedDataCommandFactory, ReceivedDataCommandFactory>();
        services.AddScoped<IReceivedDataCommand, DeleteDogCommand>();
        services.AddScoped<IReceivedDataCommand, SignUpForClassCommand>();
    }
}