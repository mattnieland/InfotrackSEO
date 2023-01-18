using InfoTrackSEO.Contexts;
using InfoTrackSEO.Extensions;
using InfoTrackSEO.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackSEO.Api.Repositories;

/// <summary>
///     A repository for working with search data
/// </summary>
public class SearchDataRepository : RepositoryBase<TrackedSearchData>, ISearchDataRepository
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public SearchDataRepository(InfoTrackContext context) : base(context)
    {
        Context = context;
    }

    private InfoTrackContext Context { get; set; }

    /// <summary>
    ///     Advanced search data search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of search data</returns>
    public IQueryable<TrackedSearchData> Search(DynamicSearch filter)
    {
        var query = Context
            .TrackedSearchData!
            .Include(d => d.Url)
            .Include(d => d.SearchTerms)
            .AsNoTracking()
            .AsQueryable();

        // apply sort & filter
        query = query.ToFilterView(filter);

        return query;
    }

    /// <summary>
    ///     Retrieve a list of objects
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    public Paging<TrackedSearchData> FindAll(int page, int size)
    {
        var total = Context.TrackedSearchData!.AsNoTracking().Count();
        var pages = (int) Math.Ceiling((double) total / size);
        var morePages = page < pages;
        var results = Context.TrackedSearchData!
            .Include(d => d.Url)
            .Include(d => d.SearchTerms)
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsQueryable();

        return new Paging<TrackedSearchData>
        {
            Page = page,
            Size = size,
            Total = total,
            Pages = pages,
            MorePages = morePages,
            Results = results
        };
    }

    /// <summary>
    ///     Cursor style paging retrieval
    /// </summary>
    /// <param name="after">The cursor value</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of objects</returns>
    public PagingCursor<TrackedSearchData> FindAllCursor(Guid? after, int size)
    {
        var total = Context.TrackedSearchData!.AsNoTracking().Count();
        TrackedSearchData? currentObject = null;
        if (after != null)
        {
            currentObject = Context.TrackedSearchData!.FirstOrDefault(x => x.UUID == after);
        }

        var results = Context.TrackedSearchData!
            .Include(d => d.Url)
            .Include(d => d.SearchTerms)
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Where(x => x.Id > (currentObject != null ? currentObject.Id : 0))
            .Take(size)
            .AsQueryable();

        return new PagingCursor<TrackedSearchData>
        {
            Cursor = new Cursor
            {
                After = results.LastOrDefault()?.UUID,
                Before = results.FirstOrDefault()?.UUID,
                Total = total
            },
            Results = results
        };
    }
}