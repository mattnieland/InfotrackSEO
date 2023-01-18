using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YAMDB.Api.Controllers;
using YAMDB.Api.Repositories;
using YAMDB.Contexts;
using YAMDB.Models;
using YAMDB.Providers;

namespace YAMDB.Tests.Controllers;

[TestClass]
public class MoviesControllerTests
{
    private readonly YAMDBContext _context;
    private readonly MoviesController _controller;

    public MoviesControllerTests()
    {
        SecretProviders.LoadSecrets();
        _context = new YAMDBContext();
        _context.Database.EnsureCreated();

        Mock<MoviesRepository> mockRepo = new(_context);
        Mock<ILogger<MoviesController>> mockLogger = new();

        _controller = new MoviesController(mockRepo.Object, mockLogger.Object);
    }

    [TestMethod]
    public void DeleteTest()
    {
        try
        {
            var movies = _context.Movies!.AsNoTracking().ToList();
            Assert.IsNotNull(movies);
            Assert.IsTrue(movies.Any());
            var movie = movies.FirstOrDefault();
            Assert.IsNotNull(movie);

            var result = _controller.Delete(movie.UUID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(NoContentResult));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    public void GetByActorIdTest()
    {
        try
        {
            var firstActor = _context
                .Actors!
                .Include(m => m.Movies)
                .AsNoTracking()
                .FirstOrDefault();
            Assert.IsNotNull(firstActor);

            var result = _controller.GetByActorId(firstActor.UUID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultMovies = okResult.Value as IQueryable<Movies>;
            Assert.IsNotNull(resultMovies);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    public void GetTest()
    {
        try
        {
            var movies = _context.Movies!.ToList();
            Assert.IsNotNull(movies);
            Assert.IsTrue(movies.Any());
            var movie = movies.FirstOrDefault();
            Assert.IsNotNull(movie);

            var result = _controller.Get(movie.UUID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultMovie = okResult.Value as Movies;
            Assert.IsNotNull(resultMovie);
            Assert.AreEqual(movie.UUID, resultMovie.UUID);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    public void ListCursorTest()
    {
        try
        {
            var result = _controller.ListCursor(null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultMovies = okResult.Value as PagingCursor<Movies>;
            Assert.IsNotNull(resultMovies);
            Assert.IsNotNull(resultMovies.Results);
            Assert.IsTrue(resultMovies.Results.Any());
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    public void ListTest()
    {
        try
        {
            var result = _controller.List();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var movies = okResult.Value as Paging<Movies>;
            Assert.IsNotNull(movies);
            Assert.IsNotNull(movies.Results);
            Assert.IsTrue(movies.Results.Any());
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    public void PostTest()
    {
        try
        {
            var movies = _context.Movies!.AsNoTracking().ToList();
            Assert.IsNotNull(movies);
            Assert.IsTrue(movies.Any());
            var highestMovieDbId = movies.Max(a => a.TheMovieDbId);

            var movie = new Movies
            {
                Title = "Test Movie",
                UUID = Guid.NewGuid(),
                TheMovieDbId = highestMovieDbId + 1
            };

            // create dummy movie
            var result = _controller.Post(movie);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // validate that the movie was created
            var createdMovies = okResult.Value as Movies;
            Assert.IsNotNull(createdMovies);
            Assert.IsTrue(createdMovies.Title == "Test Movie");
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    public void PutTest()
    {
        try
        {
            // Get current list
            var movies = _context.Movies!.ToList();
            Assert.IsNotNull(movies);
            Assert.IsTrue(movies.Any());

            // Get first movie
            var movie = movies.FirstOrDefault();
            Assert.IsNotNull(movie);

            // Update movie
            movie.Title = "Test Movie";
            var result = _controller.Put(movie.UUID, movie);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.IsTrue(objectResult.StatusCode == 201);
            var updatedMovie = objectResult.Value as Movies;
            Assert.IsNotNull(updatedMovie);
            Assert.IsTrue(updatedMovie.Title == "Test Movie");
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    //[TestMethod]
    //public void SearchTest()
    //{
    //    try
    //    {
    //        var search = new DynamicSearch
    //        {
    //            Offset = 0,
    //            Limit = 10,
    //            Sort = new List<Sort> {new() {Field = "Id", Dir = "DESC"}},
    //            Filter = new Filter
    //            {
    //                Field = "Title",
    //                Operator = "contains",
    //                Value = "The"
    //            }
    //        };
    //        var result = _controller.Search(search);
    //        Assert.IsNotNull(result);
    //        Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
    //        var okResult = result as OkObjectResult;
    //        Assert.IsNotNull(okResult);
    //        var movies = okResult.Value as IQueryable<Movies>;
    //        Assert.IsNotNull(movies);
    //        Assert.IsTrue(movies.Count() == 10);
    //        Assert.IsTrue(movies.First().Id != 1);
    //    }
    //    catch (Exception ex)
    //    {
    //        Assert.Fail(ex.Message);
    //    }
    //}
}