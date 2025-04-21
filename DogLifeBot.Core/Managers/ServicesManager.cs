using DogLiveBot.BL.Command.CommandFactory;
using DogLiveBot.BL.Command.CommandImplementation;
using DogLiveBot.BL.Command.CommandInterface;
using DogLiveBot.BL.Command.ReceivedTextCommandFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerFactory;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerImplementation;
using DogLiveBot.BL.Handlers.Messages.MessageHandlerInterface;
using DogLiveBot.BL.Services.ServiceImplementation;
using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Core.Managers.Extensions;
using DogLiveBot.Core.Options;
using DogLiveBot.Data.Context;
using DogLiveBot.Data.Repository.RepositoryImplementations;
using DogLiveBot.Data.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

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
            RegisterServices(services);
            RegisterCommands(services);
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
        services.AddDbContextFactory<ApplicationDbContext>((provider, contextOptionsBuilder) =>
        {
            var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
            contextOptionsBuilder.UseNpgsql(options.ApplicationDbConnection.ConnectionString);
        });
        services.AddScoped(typeof(IRepository<>), typeof(ApplicationRepository<>));
    }

    /// <summary>
    /// Регистрирует клиент Telegram Bot и сервис Telegram Bot в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    private static void RegisterTelegramBotClient(IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
            return new TelegramBotClient(options.TelegramBotSettings.Token);
        });
        services.AddScoped<ITelegramBotService, TelegramBotService>();
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

        services.AddScoped<IMessageHandlerFactory, MessageHandlerFactory>();

        services.AddScoped<IMessageHandler, TextMessageHandler>();
        services.AddScoped<IMessageHandler, ContactMessageHandler>();
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
    }
}