using System.Linq.Expressions;

namespace YAMDB.Api.Repositories;

/// <summary>
///     An abstraction of basic CRUD ops
/// </summary>
/// <typeparam name="T">The type of the object</typeparam>
public interface IRepositoryBase<T>
{
    /// <summary>
    ///     Create a new object
    /// </summary>
    /// <param name="entity">The object to create</param>
    void CreateAsync(T entity);

    /// <summary>
    ///     Delete an object
    /// </summary>
    /// <param name="entity">The object to delete</param>
    void DeleteAsync(T entity);

    /// <summary>
    ///     Find objects using a condition
    /// </summary>
    /// <param name="expression">The lambda expression to apply</param>
    /// <returns></returns>
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

    /// <summary>
    ///     Find an object by its id
    /// </summary>
    /// <param name="id">The object key</param>
    /// <returns></returns>
    T? FindById(int id);

    /// <summary>
    ///     Save all changes
    /// </summary>
    void SaveAsync();

    /// <summary>
    ///     Update an object
    /// </summary>
    /// <param name="entity">The updated object</param>
    void UpdateAsync(T entity);
}