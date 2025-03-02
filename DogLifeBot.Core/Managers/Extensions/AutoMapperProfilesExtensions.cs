using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DogLiveBot.Core.Managers.Extensions;

public static class AutoMapperProfilesExtensions
{
    private static readonly PropertyInfo? TypeMapActionsProperty =
        typeof(TypeMapConfiguration).GetProperty("TypeMapActions", BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly PropertyInfo? DestinationTypeDetailsProperty =
        typeof(TypeMap).GetProperty("DestinationTypeDetails", BindingFlags.NonPublic | BindingFlags.Instance);

    public static void AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            var profiles = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                    typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && !type.FullName.Contains("AutoMapper"))
                .ToList();

            foreach (var profile in profiles)
            {
                cfg.AddProfile((Profile)Activator.CreateInstance(profile)!);
            }
        });
    }
}