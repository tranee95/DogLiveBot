using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace DogLiveBot.Core.Managers.Extensions;

public static class AutoMapperProfilesExtensions
{
    public static void AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            var profiles = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && !type.FullName.Contains("AutoMapper"))
                .ToList();
            
            foreach (var profile in profiles)
            {
                cfg.AddProfile((Profile)Activator.CreateInstance(profile)!);
            }
        });
    }
}