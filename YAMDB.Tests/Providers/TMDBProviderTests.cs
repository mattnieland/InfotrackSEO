using Microsoft.VisualStudio.TestTools.UnitTesting;
using YAMDB.Providers;

namespace YAMDB.Tests.Providers;

[TestClass]
public class TMDBProviderTests
{
    [TestMethod]
    public void GetGenresTest()
    {
        var provider = new TMDBProvider();
        var genres = provider.GetGenres().Result;
        Assert.IsTrue(genres.Any());
    }

    [TestMethod]
    public void GetMovieCastTest()
    {
        var provider = new TMDBProvider();
        var topMovie = provider.GetTopMovies(1).Result.FirstOrDefault();
        Assert.IsNotNull(topMovie);

        var cast = provider.GetMovieCast(topMovie.Id!.Value).Result;
        Assert.IsTrue(cast.Any());
    }

    [TestMethod]
    public void GetTopMoviesByPageTest()
    {
        var provider = new TMDBProvider();
        var movies = provider.GetTopMovies(2).Result;
        Assert.IsTrue(movies.Any());
    }

    [TestMethod]
    public void GetTopMoviesTest()
    {
        var provider = new TMDBProvider();
        var movies = provider.GetTopMovies().Result;
        Assert.IsTrue(movies.Any());
    }

    [TestInitialize]
    public void Startup()
    {
        SecretProviders.LoadSecrets();
    }
}