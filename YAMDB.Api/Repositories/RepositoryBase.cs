using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YAMDB.Contexts;

namespace YAMDB.Api.Repositories;

/// <summary>
///     A repository for working with all objects
/// </summary>
/// <typeparam name="T">The type of the object</typeparam>
public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    /// <summary>
    ///     Initialization of the object
    /// </summary>
    /// <param name="context">A DbContext to connect to</param>
    public RepositoryBase(YAMDBContext context)
    {
        Context = context;
    }

    private YAMDBContext Context { get; set; }

    /// <summary>
    ///     Find an object by its id
    /// </summary>
    /// <param name="id">The object key</param>
    /// <returns></returns>
    public T? FindById(int id)
    {
        return Context.Set<T>().Find(id);
    }

    /// <summary>
    ///     Find objects using a condition
    /// </summary>
    /// <param name="expression">The lambda expression to apply</param>
    /// <returns></returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return Context.Set<T>().Where(expression).AsNoTracking();
    }

    /// <summary>
    ///     Create a new object
    /// </summary>
    /// <param name="entity">The object to create</param>
    public async void CreateAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
    }

    /// <summary>
    ///     Update an object
    /// </summary>
    /// <param name="entity">The updated object</param>
    public void UpdateAsync(T entity)
    {
        Context.Set<T>().Update(entity);
    }

    /// <summary>
    ///     Delete an object
    /// </summary>
    /// <param name="entity">The object to delete</param>
    public void DeleteAsync(T entity)
    {
        Context.Set<T>().Remove(entity);
    }

    /// <summary>
    ///     Save all changes
    /// </summary>
    public async void SaveAsync()
    {
        await Context.SaveChangesAsync();
    }
}