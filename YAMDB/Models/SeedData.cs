#pragma warning disable CS8618
namespace YAMDB.Models;

public class SeedData
{
    public List<Movies> Movies { get; set; }
    public List<Actors> Actors { get; set; }
    public List<ActorsInMovies> ActorsInMovies { get; set; }
    public List<MovieRatings> Ratings { get; set; }
}