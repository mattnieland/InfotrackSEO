namespace YAMDB.Api.Middleware.RateLimiting;

/// <summary>
///     A class to store statistics for rate limiting
/// </summary>
public class ClientStatistics
{
    /// <summary>
    ///     Last Successful response date and time
    /// </summary>
    public DateTime LastSuccessfulResponseTime { get; set; }

    /// <summary>
    ///     Number of requests completed successfully
    /// </summary>
    public int NumberOfRequestsCompletedSuccessfully { get; set; }
}