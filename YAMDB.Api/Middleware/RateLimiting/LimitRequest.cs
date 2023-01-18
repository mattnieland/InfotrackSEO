namespace YAMDB.Api.Middleware.RateLimiting;

/// <summary>
///     A custom attribute to enable rate limiting
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class LimitRequest : Attribute
{
    /// <summary>
    ///     The window size in seconds
    /// </summary>
    public int TimeWindow { get; set; }

    /// <summary>
    ///     The maximum number of requests allowed in the window
    /// </summary>
    public int MaxRequests { get; set; }
}