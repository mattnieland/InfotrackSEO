using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     An abstraction of the movies repository
/// </summary>
public interface IMoviesRepository : IRepositoryBase<Movies>
{
    /// <summary>
    ///     Retrieve a list of objects
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    Paging<Movies> FindAll(int page, int size);

    /// <summary>
    ///     Cursor style paging retrieval
    /// </summary>
    /// <param name="after">The cursor value</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    PagingCursor<Movies> FindAllCursor(Guid? after, int size);

    /// <summary>
    ///     Get movies by actor id
    /// </summary>
    /// <param name="actorId"></param>
    /// <returns></returns>
    IQueryable<Movies> GetByActorId(Guid actorId);

    /// <summary>
    ///     Advanced Movie search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of movies</returns>
    IQueryable<Movies> Search(DynamicSearch filter);
}