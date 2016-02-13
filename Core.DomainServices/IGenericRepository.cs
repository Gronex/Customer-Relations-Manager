using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Core.DomainServices
{
    /// <summary>
    /// Generic repository describing some generic standart calls to the database
    /// User to reduce redundancy in the other repositories, especially for tasks such as 
    /// Update, and delete where things can easily be forgotten
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T>
    {
        /// <summary>
        /// Gets all elements fulfilling the filter, ordereb by the function
        /// cut down to the fitting page size and number
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? page = null, 
            int? pageSize = null);

        /// <summary>
        /// Returns an entity with the given key, or null in none exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetByKey(params object[] key);

        /// <summary>
        /// Creates a new element
        /// </summary>
        /// <returns></returns>
        T Create();

        /// <summary>
        /// Inserts a new element in the database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Insert(T entity);

        /// <summary>
        /// Makes sure the entity with the key is no longer in the database
        /// </summary>
        /// <param name="key"></param>
        void DeleteByKey(params object[] key);

        /// <summary>
        /// Updates a model in the database
        /// </summary>
        /// <param name="updateFunction">A function updating the model, 
        /// taking the database model as the argument</param>
        /// <param name="key">Key to use for finding the element in the database</param>
        /// <returns>The updated entity, or null if it was not found</returns>
        T Update(Action<T> updateFunction, params object[] key);
    }
}
