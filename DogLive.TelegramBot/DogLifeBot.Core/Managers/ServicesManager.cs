using DogLive.TelegramBot.BL.Commands.CommandFactory;
using DogLive.TelegramBot.BL.Commands.CommandImplementation;
using DogLive.TelegramBot.BL.Commands.CommandInterface;
using DogLive.TelegramBot.BL.Commands.ReceivedDataCommandFactory;
using DogLive.TelegramBot.BL.Commands.ReceivedTextCommandFactory;
using DogLive.TelegramBot.BL.Handlers.MassTransitMessages.Consumers;
using DogLive.TelegramBot.BL.Handlers.MassTransitMessages.MassTransitMessageHandlerImplementation;
using DogLive.TelegramBot.BL.Handlers.MassTransitMessages.MassTransitMessageHandlerInterface;
using DogLive.TelegramBot.BL.Handlers.Messages.MessageHandlerFactory;
using DogLive.TelegramBot.BL.Handlers.Messages.MessageHandlerImplementation;
using DogLive.TelegramBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLive.TelegramBot.BL.Services.ServiceImplementation.Booking;
using DogLive.TelegramBot.BL.Services.ServiceImplementation.Cache;
using DogLive.TelegramBot.BL.Services.ServiceImplementation.Command;
using DogLive.TelegramBot.BL.Services.ServiceImplementation.Keyboard;
using DogLive.TelegramBot.BL.Services.ServiceImplementation.Schedule;
using DogLive.TelegramBot.BL.Services.ServiceImplementation.Telegram;
using DogLive.TelegramBot.BL.Services.ServiceImplementation.User;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Booking;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Cache;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Command;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Keyboard;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Schedule;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Telegram;
using DogLive.TelegramBot.BL.Services.ServiceInterface.User;
using DogLive.TelegramBot.Core.Managers.Extensions;
using DogLive.TelegramBot.Data.Context;
using DogLive.TelegramBot.Data.Models.Options;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Messages.Messages.Extensions;
using Shared.Messages.Messages.Schedule.Model;
using Shared.Messages.Repository.Extensions;
using Telegram.Bot;

namespace DogLive.TelegramBot.Core.Managers;

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
    /// Регистрирует контекст базы данных и репозитории в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <exception cref="ArgumentException">Отсутствует строка подключения.</exception>
    private static void RegisterDbContext(IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;

        if (string.IsNullOrEmpty(settings.ApplicationDbConnection.ConnectionString))
        {
            throw new ArgumentException("Connection string is not configured.", nameof(settings.ApplicationDbConnection.ConnectionString));
        }

        services.RegisterApplicationDbContext<TelegramBotDbContext>(settings.ApplicationDbConnection.ConnectionString);
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
    /// Регистрация massTransit.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    private static void RegisterMassTransit(IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
        
        services.RegisterMassTransit(settings.RabbitMqSettings.ConnectionString);
        services.RegisterConsumer<ScheduleConsumer>();

        services.AddScoped<IMassTransitMessageHandler<IRMQScheduleSlot>, ScheduleHandler>();
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
        services.AddScoped<IBookingService, BookingService>();

        services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();

        services.AddScoped<IMessageHandler, TextMessageHandler>();
        services.AddScoped<IMessageHandler, ContactMessageHandler>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IBookingFlowCacheService, BookingFlowCacheService>();
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
        services.AddScoped<ICommand, CreateBookingCommand>();
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
        services.AddScoped<IReceivedDataCommand, CreateBookingCommand>();
    }
}