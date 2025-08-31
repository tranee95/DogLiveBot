using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace DogLive.Scheduler.Core.Managers;

/// <summary>
/// Класс, отвечающий за создание и настройку хоста приложения.
/// </summary>
public static class ApplicationBuilderManager
{
    /// <summary>
    /// Создает и настраивает экземпляр <see cref="IHostBuilder"/> с конфигурацией приложения и логированием.
    /// </summary>
    /// <param name="args">Аргументы командной строки, переданные приложению.</param>
    /// <returns>Экземпляр <see cref="IHostBuilder"/> для настройки и запуска приложения.</returns>
    public static IHostBuilder CreateBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        builder
            .ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                var env = context.HostingEnvironment;
                configurationBuilder
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureLogging((context, loggingBuilder) =>
            {
                loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                loggingBuilder.ClearProviders();
                loggingBuilder.AddDebug();
                loggingBuilder.AddEventSourceLogger();

                if (context.HostingEnvironment.IsDevelopment())
                {
                    loggingBuilder.AddConsole();
                }

                ConfigureExtensions.AddNLog(loggingBuilder,
                    $"nlog.{context.HostingEnvironment.EnvironmentName}.config");
            })
            .UseNLog();

        return builder;
    }
}