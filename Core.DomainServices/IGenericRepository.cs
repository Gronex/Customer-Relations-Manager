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
        /// <returns></returns>
        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        IEnumerable<T> GetOrderedByStrings(
            Expression<Func<T, bool>> filter = null,
            IEnumerable<string> orderBy = null);

        PaginationEnvelope<T> GetPaged(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            int? page = null,
            int? pageSize = null,
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> findSelector = null,
            string find = null);

        PaginationEnvelope<T> GetPaged(IEnumerable<string> orderBy,
            int? page = null,
            int? pageSize = null,
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> findSelector = null,
            string find = null);

        /// <summary>
        /// Returns an entity with the given key, or null if none exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetByKey(params object[] key);
        /// <summary>
        /// Returns an entity with the given key, or throws if none exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetByKeyThrows(params object[] key);

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
        /// Removes single entity that matches selector
        /// </summary>
        /// <param name="selector">function used in single or default</param>
        void DeleteBy(Expression<Func<T, bool>> selector);

        /// <summary>
        /// Updates a model in the database
        /// </summary>
        /// <param name="updateFunction">A function updating the model, 
        /// taking the database model as the argument</param>
        /// <param name="throws">If we are to throw an exception if not found</param>
        /// <param name="key">Key to use for finding the element in the database</param>
        /// <returns>The updated entity, or null if it was not found</returns>
        T Update(Action<T> updateFunction, bool throws, params object[] key);

        /// <summary>
        /// Updates a model in the database, throws an error if not found
        /// </summary>
        /// <param name="updateFunction">A function updating the model, 
        /// taking the database model as the argument</param>
        /// <param name="key">Key to use for finding the element in the database</param>
        /// <returns>The updated entity, or null if it was not found</returns>
        T Update(Action<T> updateFunction, params object[] key);

        /// <summary>
        /// Updates a model in the database
        /// </summary>
        /// <param name="updateFunction">A function updating the model, 
        /// taking the database model as the argument</param>
        /// <param name="selector">selector to for finding the single element in the database</param>
        /// <param name="throws">If we are to throw an exception if not found</param>
        /// <returns>The updated entity, or null if it was not found</returns>
        T UpdateBy(Action<T> updateFunction, Expression<Func<T, bool>> selector, bool throws = true);

        int Count(
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> findSelector = null,
            string findTerm = null);
    }
}
