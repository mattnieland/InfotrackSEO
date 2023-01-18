using YAMDB.Api.Middleware.RateLimiting;

namespace YAMDB.Api.Extensions;

/// <summary>
///     Extensions to app builder
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    ///     Used to apply rate limiting to the application
    /// </summary>
    /// <param name="builder">The app builder</param>
    /// <returns></returns>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}