using InfoTrackSEO.Models;
using InfoTrackSEO.Providers;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackSEO.Contexts;

public class InfoTrackContext : DbContext
{
    private readonly string config = SecretProviders.GetConfig();
    public virtual DbSet<TrackedUrls>? TrackedUrls { get; set; }
    public virtual DbSet<TrackedSearchTerms>? TrackedSearch { get; set; }
    public virtual DbSet<TrackedSearchData>? TrackedSearchData { get; set; }

    public static SeedData GetSeedData()
    {
        return new SeedData
        {
            Urls = new List<TrackedUrls>(), Terms = new List<TrackedSearchTerms>(),
            SearchData = new List<TrackedSearchData>()
        };

        //var seedData = new SeedData();
        //var tmdbProvider = new TMDBProvider();
        //var genres = tmdbProvider.GetGenres().Result.ToDictionary(k => k.Id, v => v.Name);
        //var topMovies = tmdbProvider.GetTopMovies(1).Result;

        //var seedMovies = topMovies.Select((tm, index) => new Movies
        //{
        //    Id = index + 1,
        //    Title = tm.Title,
        //    Description = tm.Overview,
        //    ImageUrl = tm.PosterPath,
        //    TheMovieDbId = tm.Id!.Value,
        //    ReleaseDate = tm.ReleaseDate != null ? DateTime.Parse(tm.ReleaseDate) : null,
        //    Genres = string.Join(", ", tm.GenreIds.Select(g => genres.ContainsKey(g) ? genres[g] : null).ToList())
        //}).ToList();
        //var seedRatings = topMovies.Select((tm, index) => new MovieRatings
        //{
        //    Id = index + 1,
        //    MovieId = index + 1,
        //    Rating = tm.VoteAverage,
        //    RatingUpperLimit = 10,
        //    TotalReviews = tm.VoteCount,
        //    Source = "TheMovieDB"
        //}).ToList();

        //var seedActors = new List<Actors>();
        //var seedActorsInMovies = new List<ActorsInMovies>();
        //var movieIndex = 1;
        //var actorIndex = 1;
        //foreach (var movie in topMovies)
        //{
        //    var movieCast = tmdbProvider.GetMovieCast(movie.Id!.Value).Result;
        //    movieCast.RemoveAll(ma => seedActors.Select(sa => sa.TheMovieDbId).Contains(ma.Id));

        //    foreach (var castMember in movieCast)
        //    {
        //        var actor = new Actors
        //        {
        //            Id = actorIndex,
        //            Name = castMember.Name,
        //            TheMovieDbId = castMember.Id
        //        };
        //        seedActors.Add(actor);

        //        var actorMovie = new ActorsInMovies
        //        {
        //            ActorId = actorIndex,
        //            MovieId = movieIndex,
        //            CharacterName = castMember.Character
        //        };
        //        seedActorsInMovies.Add(actorMovie);

        //        actorIndex++;
        //    }

        //    movieIndex++;
        //}

        //seedData.Movies = seedMovies;
        //seedData.Ratings = seedRatings;
        //seedData.Actors = seedActors;
        //seedData.ActorsInMovies = seedActorsInMovies;
        //return seedData;
    }

    protected override void OnConfiguring
        (DbContextOptionsBuilder optionsBuilder)
    {
        if (config == "local")
        {
            optionsBuilder.UseInMemoryDatabase("InfoTrackDb");
        }
        else
        {
            var connString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connString))
            {
                throw new Exception("Database connection string is not set");
            }

            optionsBuilder.UseSqlServer(connString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackedSearchTerms>()
            .HasOne(t => t.Url)
            .WithMany(u => u.SearchTerms)
            .HasForeignKey(t => t.UrlId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TrackedUrls>()
            .HasMany(u => u.SearchTerms)
            .WithOne(t => t.Url)
            .HasForeignKey(t => t.UrlId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TrackedSearchData>()
            .HasOne(d => d.Url)
            .WithMany(u => u.SearchData)
            .HasForeignKey(d => d.UrlId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TrackedSearchData>()
            .HasOne(d => d.SearchTerms)
            .WithMany(u => u.SearchData)
            .HasForeignKey(d => d.TermId)
            .OnDelete(DeleteBehavior.NoAction);

        #region Seed the development database

        if (config == "local")
        {
            var seedData = GetSeedData();
            modelBuilder.Entity<TrackedUrls>().HasData(seedData.Urls);
            modelBuilder.Entity<TrackedSearchTerms>().HasData(seedData.Terms);
            modelBuilder.Entity<TrackedSearchData>().HasData(seedData.SearchData);
        }

        #endregion
    }
}