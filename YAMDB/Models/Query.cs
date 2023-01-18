using HotChocolate;
using YAMDB.Contexts;

namespace YAMDB.Models;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Actors> GetActors([Service] YAMDBContext context)
    {
        return context.Actors!;
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Movies> GetMovies([Service] YAMDBContext context)
    {
        return context.Movies!;
    }
}