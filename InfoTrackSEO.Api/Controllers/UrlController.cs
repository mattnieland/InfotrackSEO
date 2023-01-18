using InfoTrackSEO.Api.Middleware.RateLimiting;
using InfoTrackSEO.Api.Repositories;
using InfoTrackSEO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InfoTrackSEO.Api.Controllers;

/// <summary>
///     Endpoints to create/retrieve/update/delete urls
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class UrlController : ControllerBase
{
    private readonly ILogger<UrlController> _logger;
    private readonly IUrlsRepository _repository;

    /// <summary>
    ///     The controller for actor endpoints
    /// </summary>
    /// <param name="repository">A repository for working with our data</param>
    /// <param name="logger">Our default logger</param>
    public UrlController(IUrlsRepository repository, ILogger<UrlController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    ///     Delete an URL
    /// </summary>
    /// <param name="uuid">The unique GUID for the URL</param>
    /// <returns>204 Content</returns>
    [HttpDelete]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:urls")]
    public IActionResult Delete(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingUrl = _repository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingUrl == null)
            {
                return NotFound();
            }

            // delete and save
            _repository.DeleteAsync(existingUrl);
            _repository.SaveAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    ///     Get a specific URL
    /// </summary>
    /// <param name="uuid">The unique GUID for the URL</param>
    /// <returns>An actor object</returns>
    [HttpGet("{uuid:guid}")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "An URL object", typeof(TrackedUrls))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:urls")]
    public IActionResult Get(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingURL = _repository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingURL == null)
            {
                return NotFound();
            }

            return new OkObjectResult(existingURL);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Retrieve all URLs
    /// </summary>
    /// <returns>A list of URLs</returns>
    [HttpGet]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of URLs", typeof(IEnumerable<TrackedUrls>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:urls")]
    public IActionResult List([FromQuery] int? page = 1, [FromQuery] int? size = 50)
    {
        try
        {
            var results = _repository.FindAll(page!.Value, size!.Value);
            return new OkObjectResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Retrieve all URLs
    /// </summary>
    /// <returns>A list of URLs</returns>
    [HttpGet("cursor")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of URLs", typeof(IEnumerable<TrackedUrls>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:urls")]
    public IActionResult ListCursor([FromQuery] Guid? after, [FromQuery] int? size = 50)
    {
        try
        {
            var results = _repository.FindAllCursor(after, size!.Value);
            return new OkObjectResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Create a URL
    /// </summary>
    /// <param name="url">A URL object</param>
    /// <returns>A url object</returns>
    [HttpPost]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "A url object", typeof(TrackedUrls))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The url already exists")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:urls")]
    public IActionResult Post([FromBody] TrackedUrls url)
    {
        try
        {
            // check that the url is valid
            if (string.IsNullOrEmpty(url.Url))
            {
                return BadRequest();
            }

            // check that the object exists
            var existingURL = _repository.FindByCondition(a => a.UUID == url.UUID).FirstOrDefault();
            if (existingURL != null)
            {
                return Conflict();
            }

            existingURL = _repository.FindById(url.Id);
            if (existingURL != null)
            {
                return Conflict();
            }

            // create and save
            _repository.CreateAsync(url);
            _repository.SaveAsync();
            return new OkObjectResult(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Update an URL object
    /// </summary>
    /// <param name="uuid">The unique GUID for the URL</param>
    /// <param name="url">The new URL object</param>
    /// <returns>201 Created</returns>
    [HttpPut("{uuid:guid}")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The URL does not exist")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:urls")]
    public IActionResult Put(Guid uuid, [FromBody] TrackedUrls url)
    {
        try
        {
            // check that the url is valid
            if (string.IsNullOrEmpty(url.Url))
            {
                return BadRequest();
            }

            // check that the object exists
            var existingUrl = _repository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingUrl == null)
            {
                return NotFound();
            }

            // update and save
            _repository.UpdateAsync(url);
            _repository.SaveAsync();
            return new ObjectResult(url) {StatusCode = StatusCodes.Status201Created};
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }


    /// <summary>
    ///     Advanced search for urls
    /// </summary>
    /// <param name="search">The filter parameters</param>
    /// <returns>A list of urls</returns>
    [HttpPost("search")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of urls", typeof(IEnumerable<TrackedUrls>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:urls")]
    public IActionResult Search([FromBody] DynamicSearch search)
    {
        try
        {
            var results = _repository.Search(search);
            return new OkObjectResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}