using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;
using YAMDB.Extensions;
using YAMDB.Models;

namespace YAMDB.Api.Repositories;

/// <summary>
///     A repository for working with actors
/// </summary>
public class ActorsRepository : RepositoryBase<Actors>, IActorsRepository
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public ActorsRepository(YAMDBContext context) : base(context)
    {
        Context = context;
    }

    private YAMDBContext Context { get; set; }

    /// <summary>
    ///     Get actors by movie id
    /// </summary>
    /// <param name="movieId">The unique GUID for the movie</param>
    /// <returns>A list of actors</returns>
    public IQueryable<Actors> GetByMovieId(Guid movieId)
    {
        var movie = Context.Movies?
            .FirstOrDefault(m => m.UUID == movieId);
        if (movie == null)
        {
            return Enumerable.Empty<Actors>().AsQueryable();
        }

        var actorMovies = Context
            .ActorsInMovies?
            .Include(a => a.Actor)
            .Where(aim => aim.MovieId == movie.Id);

        if (actorMovies == null)
        {
            return Enumerable.Empty<Actors>().AsQueryable();
        }

        return actorMovies.Select(aim => aim.Actor!);
    }

    /// <summary>
    ///     Advanced actor search
    /// </summary>
    /// <param name="filter">filter object</param>
    /// <returns>A list of actors</returns>
    public IQueryable<Actors> Search(DynamicSearch filter)
    {
        var query = Context
            .Actors!
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
    public Paging<Actors> FindAll(int page, int size)
    {
        var total = Context.Actors!.AsNoTracking().Count();
        var pages = (int) Math.Ceiling((double) total / size);
        var morePages = page < pages;
        var results = Context.Actors!
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsQueryable();

        return new Paging<Actors>
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
    public PagingCursor<Actors> FindAllCursor(Guid? after, int size)
    {
        var total = Context.Actors!.AsNoTracking().Count();
        Actors? currentObject = null;
        if (after != null)
        {
            currentObject = Context.Actors!.FirstOrDefault(x => x.UUID == after);
        }

        var results = Context.Actors!
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Where(x => x.Id > (currentObject != null ? currentObject.Id : 0))
            .Take(size)
            .AsQueryable();

        return new PagingCursor<Actors>
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