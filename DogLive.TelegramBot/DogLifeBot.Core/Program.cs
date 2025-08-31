using DogLive.TelegramBot.BL.Services.ServiceInterface.Telegram;
using DogLive.TelegramBot.Core.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DogLive.TelegramBot.Core;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = ApplicationBuilderManager.CreateBuilder(args);
        ServicesManager.ConfigureServices(builder);

        var host = builder.Build();

        using (var scope = host.Services.CreateScope())
        {
            using (var cts = new CancellationTokenSource())
            {
                var telegramBotService = scope.ServiceProvider.GetRequiredService<ITelegramBotService>();
                await telegramBotService.Start(cts.Token);
            }
        }

        await host.RunAsync();
    }
}



