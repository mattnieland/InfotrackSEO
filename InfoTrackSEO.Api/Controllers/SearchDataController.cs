using InfoTrackSEO.Api.Middleware.RateLimiting;
using InfoTrackSEO.Api.Repositories;
using InfoTrackSEO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InfoTrackSEO.Api.Controllers;

/// <summary>
///     Endpoints to manage search data
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class SearchDataController : ControllerBase
{
    private readonly ISearchDataRepository _dataRepository;
    private readonly ILogger<SearchDataController> _logger;

    /// <summary>
    ///     The controller for search data endpoints
    /// </summary>
    /// <param name="dataRepository">A repository for working with our data</param>
    /// <param name="logger">Our default logger</param>
    public SearchDataController(ISearchDataRepository dataRepository, ILogger<SearchDataController> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    /// <summary>
    ///     Retrieve all search data
    /// </summary>
    /// <returns>A list of search data</returns>
    [HttpGet]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of search data", typeof(IEnumerable<TrackedSearchData>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:searches")]
    public IActionResult List([FromQuery] int? page = 1, [FromQuery] int? size = 50)
    {
        try
        {
            var results = _dataRepository.FindAll(page!.Value, size!.Value);
            return new OkObjectResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Retrieve all search data
    /// </summary>
    /// <returns>A list of search data</returns>
    [HttpGet("cursor")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of search data", typeof(IEnumerable<TrackedSearchData>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:searches")]
    public IActionResult ListCursor([FromQuery] Guid? after, [FromQuery] int? size = 50)
    {
        try
        {
            var results = _dataRepository.FindAllCursor(after, size!.Value);
            return new OkObjectResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Advanced search for search data
    /// </summary>
    /// <param name="search">The filter parameters</param>
    /// <returns>A list of search data</returns>
    [HttpPost("search")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of search data", typeof(IEnumerable<TrackedSearchData>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:searches")]
    public IActionResult Search([FromBody] DynamicSearch search)
    {
        try
        {
            var results = _dataRepository.Search(search);
            return new OkObjectResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}