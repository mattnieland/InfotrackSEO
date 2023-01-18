using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     An abstraction of the actors repository
/// </summary>
public interface IActorsRepository : IRepositoryBase<Actors>
{
    /// <summary>
    ///     Retrieve a list of objects
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    Paging<Actors> FindAll(int page, int size);

    /// <summary>
    ///     Cursor style paging retrieval
    /// </summary>
    /// <param name="after">The cursor value</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    PagingCursor<Actors> FindAllCursor(Guid? after, int size);

    /// <summary>
    ///     Get actors by movie id
    /// </summary>
    /// <param name="movieId">The unique GUID for the movie</param>
    /// <returns></returns>
    IQueryable<Actors> GetByMovieId(Guid movieId);

    /// <summary>
    ///     Advanced actor search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of actors</returns>
    IQueryable<Actors> Search(DynamicSearch filter);
}