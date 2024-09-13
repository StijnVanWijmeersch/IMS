using IMS.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace IMS.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, bool> CacheKeys = [];
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? cachedValue = await _distributedCache.GetStringAsync(key);

        if (cachedValue is null)
        {
            return null;
        }

        T? deserializedValue = JsonConvert.DeserializeObject<T>(cachedValue);

        return deserializedValue;
    }

    public async Task<T> GetAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default) where T : class
    {
        T? cachedValue = await GetAsync<T>(key, cancellationToken);

        if (cachedValue is not null)
        {
            return cachedValue;
        }

        cachedValue = await factory();

        await SetAsync(key, cachedValue, cancellationToken);

        return cachedValue;
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        string serializedValue = JsonConvert.SerializeObject(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);

        CacheKeys.TryAdd(key, false);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);

        CacheKeys.TryRemove(key, out bool _);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        IEnumerable<Task> tasks = CacheKeys.Keys
            .Where(x => x.StartsWith(prefix))
            .Select(x => RemoveAsync(x, cancellationToken));

        await Task.WhenAll(tasks);
    }


}
