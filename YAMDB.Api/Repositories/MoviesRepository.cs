using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;
using YAMDB.Extensions;
using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     A repository for working with actors
/// </summary>
public class MoviesRepository : RepositoryBase<Movies>, IMoviesRepository
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public MoviesRepository(YAMDBContext context) : base(context)
    {
        Context = context;
    }

    private YAMDBContext Context { get; set; }

    /// <summary>
    ///     Get movies by actor id
    /// </summary>
    /// <param name="actorId">The unique GUID for the actor</param>
    /// <returns>A list of movies</returns>
    public IQueryable<Movies> GetByActorId(Guid actorId)
    {
        var actor = Context.Actors?
            .AsNoTracking()
            .FirstOrDefault(m => m.UUID == actorId);
        if (actor == null)
        {
            return Enumerable.Empty<Movies>().AsQueryable();
        }

        var actorMovies = Context
            .ActorsInMovies?
            .AsNoTracking()
            .Include(a => a.Movie)
            .Where(aim => aim.ActorId == actor.Id);

        if (actorMovies == null)
        {
            return Enumerable.Empty<Movies>().AsQueryable();
        }

        return actorMovies.Select(aim => aim.Movie!);
    }

    /// <summary>
    ///     Advanced Movie search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of movies</returns>
    public IQueryable<Movies> Search(DynamicSearch filter)
    {
        var query = Context
            .Movies!
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
    public Paging<Movies> FindAll(int page, int size)
    {
        var total = Context.Movies!.AsNoTracking().Count();
        var pages = (int) Math.Ceiling((double) total / size);
        var morePages = page < pages;
        var results = Context.Movies!
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsQueryable();

        return new Paging<Movies>
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
    public PagingCursor<Movies> FindAllCursor(Guid? after, int size)
    {
        var total = Context.Movies!.AsNoTracking().Count();
        Movies? currentObject = null;
        if (after != null)
        {
            currentObject = Context.Movies!.FirstOrDefault(x => x.UUID == after);
        }

        var results = Context.Movies!
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Where(x => x.Id > (currentObject != null ? currentObject.Id : 0))
            .Take(size)
            .AsQueryable();

        return new PagingCursor<Movies>
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