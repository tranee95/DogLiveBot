using DogLive.TelegramBot.BL.Services.ServiceInterface.Cache;
using DogLive.TelegramBot.BL.Services.ServiceInterface;
using DogLive.TelegramBot.Data.Models.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DogLive.TelegramBot.BL.Services.ServiceImplementation.Cache;

public class CacheService : ICacheService
{
    private readonly TimeSpan liveCacheTime;
    private readonly IDistributedCache _cache;

    
    public CacheService(
        IOptions<ApplicationOptions> options, 
        IDistributedCache cache)
    {
        _cache = cache;
        liveCacheTime = TimeSpan.FromMinutes(options.Value.RedisSettings.LiveTimeMinutes);
    }

    /// <inheritdoc />
    public async Task Set(string key, string data, CancellationToken cancellationToken)
    {
        await _cache.SetStringAsync(key, data, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = liveCacheTime
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> Get(string key, CancellationToken cancellationToken)
    {
       return await _cache.GetStringAsync(key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task Remove(string key, CancellationToken cancellationToke)
    {
        await _cache.RemoveAsync(key, cancellationToke);
    }
}