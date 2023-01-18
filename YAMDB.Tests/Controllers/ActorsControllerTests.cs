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
public class ActorsControllerTests
{
    private readonly YAMDBContext _context;
    private readonly ActorsController _controller;

    public ActorsControllerTests()
    {
        SecretProviders.LoadSecrets();
        _context = new YAMDBContext();
        _context.Database.EnsureCreated();

        Mock<ActorsRepository> mockRepo = new(_context);
        Mock<ILogger<ActorsController>> mockLogger = new();

        _controller = new ActorsController(mockRepo.Object, mockLogger.Object);
    }

    [TestMethod]
    public void DeleteTest()
    {
        try
        {
            var actors = _context.Actors!.AsNoTracking().ToList();
            Assert.IsNotNull(actors);
            Assert.IsTrue(actors.Any());
            var actor = actors.FirstOrDefault();
            Assert.IsNotNull(actor);

            var result = _controller.Delete(actor.UUID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(NoContentResult));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    public void GetByMovieIdTest()
    {
        try
        {
            var firstMovie = _context
                .Movies!
                .Include(m => m.Actors)
                .FirstOrDefault();
            Assert.IsNotNull(firstMovie);

            var result = _controller.GetByMovieId(firstMovie.UUID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultActors = okResult.Value as IQueryable<Actors>;
            Assert.IsNotNull(resultActors);
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
            var actors = _context.Actors!.ToList();
            Assert.IsNotNull(actors);
            Assert.IsTrue(actors.Any());
            var actor = actors.FirstOrDefault();
            Assert.IsNotNull(actor);

            var result = _controller.Get(actor.UUID);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultActor = okResult.Value as Actors;
            Assert.IsNotNull(resultActor);
            Assert.AreEqual(actor.UUID, resultActor.UUID);
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
            var resultActors = okResult.Value as PagingCursor<Actors>;
            Assert.IsNotNull(resultActors);
            Assert.IsNotNull(resultActors.Results);
            Assert.IsTrue(resultActors.Results.Any());
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
            var actors = okResult.Value as Paging<Actors>;
            Assert.IsNotNull(actors);
            Assert.IsNotNull(actors.Results);
            Assert.IsTrue(actors.Results.Any());
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
            var actors = _context.Actors!.AsNoTracking().ToList();
            Assert.IsNotNull(actors);
            Assert.IsTrue(actors.Any());
            var highestMovieDbId = actors.Max(a => a.TheMovieDbId);

            var actor = new Actors
            {
                Name = "Test Actor",
                UUID = Guid.NewGuid(),
                TheMovieDbId = highestMovieDbId + 1
            };

            // create dummy actor
            var result = _controller.Post(actor);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            // validate that the actor was created
            var createdActor = okResult.Value as Actors;
            Assert.IsNotNull(createdActor);
            Assert.IsTrue(createdActor.Name == "Test Actor");
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
            var actors = _context.Actors!.ToList();
            Assert.IsNotNull(actors);
            Assert.IsTrue(actors.Any());

            // Get first actor
            var actor = actors.FirstOrDefault();
            Assert.IsNotNull(actor);

            // Update actor
            actor.Name = "Test Actor";
            var result = _controller.Put(actor.UUID, actor);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.IsTrue(objectResult.StatusCode == 201);
            var updatedActor = objectResult.Value as Actors;
            Assert.IsNotNull(updatedActor);
            Assert.IsTrue(updatedActor.Name == "Test Actor");
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
    //            Sort = new List<Sort> { new() { Field = "Id", Dir = "DESC" } }
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