using InfoTrackSEO.Contexts;
using InfoTrackSEO.Extensions;
using InfoTrackSEO.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackSEO.Api.Repositories;

/// <summary>
///     A repository for working with urls
/// </summary>
public class UrlsRepository : RepositoryBase<TrackedUrls>, IUrlsRepository
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public UrlsRepository(InfoTrackContext context) : base(context)
    {
        Context = context;
    }

    private InfoTrackContext Context { get; set; }

    /// <summary>
    ///     Advanced url search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of urls</returns>
    public IQueryable<TrackedUrls> Search(DynamicSearch filter)
    {
        var query = Context
            .TrackedUrls!
            .Include(u => u.SearchTerms)
            .AsNoTracking()
            .AsQueryable();

        // apply sort & filter
        query = query.ToFilterView(filter);

        return query;
    }

    /// <summary>
    ///     Retrieve a list of urls
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of urls</returns>
    public Paging<TrackedUrls> FindAll(int page, int size)
    {
        var total = Context.TrackedUrls!.AsNoTracking().Count();
        var pages = (int) Math.Ceiling((double) total / size);
        var morePages = page < pages;
        var results = Context.TrackedUrls!
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsQueryable();

        return new Paging<TrackedUrls>
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
    /// <returns>The list of urls</returns>
    public PagingCursor<TrackedUrls> FindAllCursor(Guid? after, int size)
    {
        var total = Context.TrackedUrls!.AsNoTracking().Count();
        TrackedUrls? currentObject = null;
        if (after != null)
        {
            currentObject = Context.TrackedUrls!.FirstOrDefault(x => x.UUID == after);
        }

        var results = Context.TrackedUrls!
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Where(x => x.Id > (currentObject != null ? currentObject.Id : 0))
            .Take(size)
            .AsQueryable();

        return new PagingCursor<TrackedUrls>
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