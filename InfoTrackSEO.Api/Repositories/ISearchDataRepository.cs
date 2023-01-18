using InfoTrackSEO.Models;

namespace InfoTrackSEO.Api.Repositories;

/// <summary>
///     An abstraction of the search data repository
/// </summary>
public interface ISearchDataRepository : IRepositoryBase<TrackedSearchData>
{
    /// <summary>
    ///     Retrieve a list of objects
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    Paging<TrackedSearchData> FindAll(int page, int size);

    /// <summary>
    ///     Cursor style paging retrieval
    /// </summary>
    /// <param name="after">The cursor value</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    PagingCursor<TrackedSearchData> FindAllCursor(Guid? after, int size);

    /// <summary>
    ///     Advanced search data search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of search data</returns>
    IQueryable<TrackedSearchData> Search(DynamicSearch filter);
}