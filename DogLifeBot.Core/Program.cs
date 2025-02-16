using DogLiveBot.BL.ServiceInterface;
using DogLiveBot.Core.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DogLiveBot.Core;

class Program
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



