using InfoTrackSEO.Models;

namespace InfoTrackSEO.Api.Repositories;

/// <summary>
///     An abstraction of the search term repository
/// </summary>
public interface ITermsRepository : IRepositoryBase<TrackedSearchTerms>
{
    /// <summary>
    ///     Retrieve a list of terms
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of terms</returns>
    Paging<TrackedSearchTerms> FindAll(int page, int size);

    /// <summary>
    ///     Cursor style paging retrieval
    /// </summary>
    /// <param name="after">The cursor value</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of terms</returns>
    PagingCursor<TrackedSearchTerms> FindAllCursor(Guid? after, int size);

    /// <summary>
    ///     Advanced terms search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of terms</returns>
    IQueryable<TrackedSearchTerms> Search(DynamicSearch filter);
}