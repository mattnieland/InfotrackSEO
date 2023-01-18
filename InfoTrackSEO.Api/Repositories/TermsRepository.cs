using InfoTrackSEO.Contexts;
using InfoTrackSEO.Extensions;
using InfoTrackSEO.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackSEO.Api.Repositories;

/// <summary>
///     A repository for working with urls
/// </summary>
public class TermsRepository : RepositoryBase<TrackedSearchTerms>, ITermsRepository
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public TermsRepository(InfoTrackContext context) : base(context)
    {
        Context = context;
    }

    private InfoTrackContext Context { get; set; }

    /// <summary>
    ///     Advanced terms search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of terms</returns>
    public IQueryable<TrackedSearchTerms> Search(DynamicSearch filter)
    {
        var query = Context
            .TrackedSearch!
            .Include(u => u.Url)
            .AsNoTracking()
            .AsQueryable();

        // apply sort & filter
        query = query.ToFilterView(filter);

        return query;
    }

    /// <summary>
    ///     Retrieve a list of terms
    /// </summary>
    /// <param name="page">Page to return</param>
    /// <param name="size">The page size</param>
    /// <returns>The list of terms</returns>
    public Paging<TrackedSearchTerms> FindAll(int page, int size)
    {
        var total = Context.TrackedSearch!.AsNoTracking().Count();
        var pages = (int) Math.Ceiling((double) total / size);
        var morePages = page < pages;
        var results = Context.TrackedSearch!
            .Include(u => u.Url)
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsQueryable();

        return new Paging<TrackedSearchTerms>
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
    /// <returns>The list of terms</returns>
    public PagingCursor<TrackedSearchTerms> FindAllCursor(Guid? after, int size)
    {
        var total = Context.TrackedSearch!.AsNoTracking().Count();
        TrackedSearchTerms? currentObject = null;
        if (after != null)
        {
            currentObject = Context.TrackedSearch!.FirstOrDefault(x => x.UUID == after);
        }

        var results = Context.TrackedSearch!
            .Include(u => u.Url)
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Where(x => x.Id > (currentObject != null ? currentObject.Id : 0))
            .Take(size)
            .AsQueryable();

        return new PagingCursor<TrackedSearchTerms>
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