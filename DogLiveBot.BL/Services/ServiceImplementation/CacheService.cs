using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DogLiveBot.BL.Services.ServiceImplementation;

public class CacheService : ICacheService
{
    private readonly IOptions<ApplicationOptions> _options;
    private readonly TimeSpan liveCacheTime;
    private readonly IDistributedCache _cache;

    
    public CacheService(
        IOptions<ApplicationOptions> options, 
        IDistributedCache cache)
    {
        _options = options;
        _cache = cache;
        
        liveCacheTime = TimeSpan.FromMinutes(_options.Value.RedisSettings.LiveTimeMinutes);
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