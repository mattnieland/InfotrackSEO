using InfoTrackSEO.Api.Middleware.RateLimiting;
using InfoTrackSEO.Api.Repositories;
using InfoTrackSEO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InfoTrackSEO.Api.Controllers;

/// <summary>
///     Endpoints to create/retrieve/update/delete search terms
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class TermsController : ControllerBase
{
    private readonly ILogger<TermsController> _logger;
    private readonly ITermsRepository _repository;

    /// <summary>
    ///     The controller for actor endpoints
    /// </summary>
    /// <param name="repository">A repository for working with our data</param>
    /// <param name="logger">Our default logger</param>
    public TermsController(ITermsRepository repository, ILogger<TermsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    ///     Delete a search terms
    /// </summary>
    /// <param name="uuid">The unique GUID for the search terms</param>
    /// <returns>204 Content</returns>
    [HttpDelete]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:terms")]
    public IActionResult Delete(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingTerm = _repository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingTerm == null)
            {
                return NotFound();
            }

            // delete and save
            _repository.DeleteAsync(existingTerm);
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
    ///     Get a specific search terms
    /// </summary>
    /// <param name="uuid">The unique GUID for the search terms</param>
    /// <returns>An search terms object</returns>
    [HttpGet("{uuid:guid}")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "An search terms object", typeof(TrackedSearchTerms))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:terms")]
    public IActionResult Get(Guid uuid)
    {
        try
        {
            // check that the object exists
            var existingTerm = _repository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingTerm == null)
            {
                return NotFound();
            }

            return new OkObjectResult(existingTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Retrieve all search terms
    /// </summary>
    /// <returns>A list of search terms</returns>
    [HttpGet]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of search terms", typeof(IEnumerable<TrackedSearchTerms>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:terms")]
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
    ///     Retrieve all search terms
    /// </summary>
    /// <returns>A list of search terms</returns>
    [HttpGet("cursor")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of search terms", typeof(IEnumerable<TrackedSearchTerms>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:terms")]
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
    ///     Create a search terms
    /// </summary>
    /// <param name="term">A search terms object</param>
    /// <returns>A search terms object</returns>
    [HttpPost]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "A search terms object", typeof(TrackedSearchTerms))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The search terms already exists")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:terms")]
    public IActionResult Post([FromBody] TrackedSearchTerms term)
    {
        try
        {
            // check that the term is valid
            if (string.IsNullOrEmpty(term.Term))
            {
                return BadRequest();
            }


            // check that the object exists
            var existingTerm = _repository.FindByCondition(a => a.UUID == term.UUID).FirstOrDefault();
            if (existingTerm != null)
            {
                return Conflict();
            }

            existingTerm = _repository.FindById(term.Id);
            if (existingTerm != null)
            {
                return Conflict();
            }

            // create and save
            _repository.CreateAsync(term);
            _repository.SaveAsync();
            return new OkObjectResult(term);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    /// <summary>
    ///     Update an term object
    /// </summary>
    /// <param name="uuid">The unique GUID for the term</param>
    /// <param name="term">The new term object</param>
    /// <returns>201 Created</returns>
    [HttpPut("{uuid:guid}")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The term does not exist")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("write:terms")]
    public IActionResult Put(Guid uuid, [FromBody] TrackedSearchTerms term)
    {
        try
        {
            // check that the term is valid
            if (string.IsNullOrEmpty(term.Term))
            {
                return BadRequest();
            }

            // check that the object exists
            var existingTerm = _repository.FindByCondition(a => a.UUID == uuid).FirstOrDefault();
            if (existingTerm == null)
            {
                return NotFound();
            }

            // update and save
            _repository.UpdateAsync(term);
            _repository.SaveAsync();
            return new ObjectResult(term) {StatusCode = StatusCodes.Status201Created};
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }


    /// <summary>
    ///     Advanced search for terms
    /// </summary>
    /// <param name="search">The filter parameters</param>
    /// <returns>A list of terms</returns>
    [HttpPost("search")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    [SwaggerResponse(StatusCodes.Status200OK, "List of terms", typeof(IEnumerable<TrackedSearchTerms>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Service Unavailable")]
    [Authorize("read:terms")]
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