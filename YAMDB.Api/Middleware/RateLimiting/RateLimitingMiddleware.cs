using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using YAMDB.Api.Extensions;

namespace YAMDB.Api.Middleware.RateLimiting;

/// <summary>
///     A Custom middleware to limit the number of requests
///     Credit to https://www.c-sharpcorner.com/blogs/rate-limiting-in-net-60
///     For design and implementation
/// </summary>
public class RateLimitingMiddleware
{
    private readonly IDistributedCache _cache;
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Inject the cache and the next middleware
    /// </summary>
    /// <param name="next">The next request delegate</param>
    /// <param name="cache">The cache object</param>
    public RateLimitingMiddleware(RequestDelegate next,
        IDistributedCache cache)
    {
        _next = next;
        _cache = cache;
    }

    /// <summary>
    ///     Invoked on each request
    /// </summary>
    /// <param name="context">The request context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        // read the LimitRequest attribute from the endpoint
        var rateLimitDecorator = endpoint?.Metadata.GetMetadata<LimitRequest>();
        if (rateLimitDecorator is null)
        {
            await _next(context);
            return;
        }

        var key = GenerateClientKey(context);
        var _clientStatistics = GetClientStatisticsByKey(key).Result;
        // Check whether the request violates the rate limit policy
        if (_clientStatistics != null &&
            DateTime.Now < _clientStatistics.LastSuccessfulResponseTime.AddSeconds(rateLimitDecorator.TimeWindow) &&
            _clientStatistics.NumberOfRequestsCompletedSuccessfully == rateLimitDecorator.MaxRequests)
        {
            context.Response.StatusCode = (int) HttpStatusCode.TooManyRequests;
            return;
        }

        await UpdateClientStatisticsAsync(key, rateLimitDecorator.MaxRequests);
        await _next(context);
    }

    /// <summary>
    ///     generate ClientKey from the context
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static string GenerateClientKey(HttpContext context)
    {
        return $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";
    }

    /// <summary>
    ///     Get the client statistics from caching
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private async Task<ClientStatistics> GetClientStatisticsByKey(string key)
    {
        return await _cache.GetCachedValueAsync<ClientStatistics>(key);
    }

    private async Task UpdateClientStatisticsAsync(string key, int maxRequests)
    {
        var _clientStats = _cache.GetCachedValueAsync<ClientStatistics>(key).Result;
        if (_clientStats is not null)
        {
            _clientStats.LastSuccessfulResponseTime = DateTime.UtcNow;
            if (_clientStats.NumberOfRequestsCompletedSuccessfully != maxRequests)
            {
                _clientStats.NumberOfRequestsCompletedSuccessfully++;
            }
            else
            {
                _clientStats.NumberOfRequestsCompletedSuccessfully = 1;
            }

            await _cache.SetCachedValueAsync(key, _clientStats);
        }
        else
        {
            var clientStats = new ClientStatistics
            {
                LastSuccessfulResponseTime = DateTime.UtcNow,
                NumberOfRequestsCompletedSuccessfully = 1
            };

            await _cache.SetCachedValueAsync(key, clientStats);
        }
    }
}