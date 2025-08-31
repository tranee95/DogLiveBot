using DogLive.Scheduler.Core.Managers;
using Microsoft.Extensions.Hosting;

namespace DogLive.Scheduler.Core;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = ApplicationBuilderManager.CreateBuilder(args);
        ServicesManager.ConfigureServices(builder);

        var host = builder.Build();
        await host.RunAsync();
    }
}



