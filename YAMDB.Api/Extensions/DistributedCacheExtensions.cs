using Microsoft.Extensions.Caching.Distributed;

#pragma warning disable CS8604

namespace YAMDB.Api.Extensions;

/// <summary>
///     Extensions for working with the cache
/// </summary>
public static class DistributedCacheExtensions
{
    /// <summary>
    ///     Get cached value
    /// </summary>
    /// <param name="cache">The cache</param>
    /// <param name="key">The item key</param>
    /// <param name="token">Cancellation token</param>
    /// <typeparam name="T">The object type</typeparam>
    /// <returns></returns>
    public static async Task<T> GetCachedValueAsync<T>(this IDistributedCache cache, string key,
        CancellationToken token = default) where T : class
    {
        var result = await cache.GetAsync(key, token);
        return result.FromByteArray<T>();
    }

    /// <summary>
    ///     Set a cached value
    /// </summary>
    /// <param name="cache">The cache</param>
    /// <param name="key">The item key</param>
    /// <param name="value">The object to store</param>
    /// <param name="token">Cancellation token</param>
    /// <typeparam name="T">The object type</typeparam>
    public static async Task SetCachedValueAsync<T>(this IDistributedCache cache, string key, T value,
        CancellationToken token = default)
    {
        await cache.SetAsync(key, value.ToByteArray(), token);
    }
}