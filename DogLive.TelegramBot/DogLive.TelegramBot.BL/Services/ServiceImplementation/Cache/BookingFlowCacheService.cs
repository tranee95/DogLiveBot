using System.Text.Json;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Cache;
using DogLive.TelegramBot.Data.Models.Cache;
using Microsoft.Extensions.Caching.Distributed;

namespace DogLive.TelegramBot.BL.Services.ServiceImplementation.Cache;

public class BookingFlowCacheService : IBookingFlowCacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public BookingFlowCacheService(IDistributedCache cache) => _cache = cache;

    /// <inheritdoc/>
    public async Task<BookingFlowCache> GetAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        var key = BookingCacheKeys.ForUser(telegramUserId);
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes is null)
        {
            return new BookingFlowCache();
        }

        return JsonSerializer.Deserialize<BookingFlowCache>(bytes, JsonOptions) ?? new BookingFlowCache();
    }

    /// <inheritdoc/>
    public async Task SetAsync(long telegramUserId, BookingFlowCache state, TimeSpan ttl,
        CancellationToken cancellationToken)
    {
        var key = BookingCacheKeys.ForUser(telegramUserId);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(state, JsonOptions);

        await _cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task ClearAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(BookingCacheKeys.ForUser(telegramUserId), cancellationToken);
    }
}