using InfoTrackSEO.Models;

namespace InfoTrackSEO.Api.Repositories;

/// <summary>
///     An abstraction of the urls repository
/// </summary>
public interface IUrlsRepository : IRepositoryBase<TrackedUrls>
{
    /// <summary>
    ///     Retrieve a list of urls
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of urls</returns>
    Paging<TrackedUrls> FindAll(int page, int size);

    /// <summary>
    ///     Cursor style paging retrieval
    /// </summary>
    /// <param name="after">The cursor value</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of urls</returns>
    PagingCursor<TrackedUrls> FindAllCursor(Guid? after, int size);

    /// <summary>
    ///     Advanced url search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of url</returns>
    IQueryable<TrackedUrls> Search(DynamicSearch filter);
}