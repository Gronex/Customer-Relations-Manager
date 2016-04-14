using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Core.DomainServices;
using Infrastructure.DataAccess.Exceptions;
using System.Linq.Dynamic;
using Infrastructure.DataAccess.Extentions;

namespace Infrastructure.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly IApplicationContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(IApplicationContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            return FilterLogic(filter, orderBy, null, null);
        }

        public IEnumerable<T> GetOrderedByStrings(Expression<Func<T, bool>> filter = null, IEnumerable<string> orderBy = null)
        {
            return FilterLogic(filter, orderBy, null, null);
        }

        public PaginationEnvelope<T> GetPaged(
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
            int? page = null, 
            int? pageSize = null, 
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> findSelector = null,
            string findTerm = null)
        {
            var data = FilterLogic(filter, orderBy, page, pageSize, findSelector, findTerm);
            
            return new PaginationEnvelope<T>
            {
                PageSize = pageSize ?? -1,
                PageNumber = page ?? -1,
                ItemCount = Count(filter, findSelector, findTerm),
                Data = data
            };
        }

        public PaginationEnvelope<T> GetPaged(
            IEnumerable<string> orderBy, 
            int? page = null, 
            int? pageSize = null, 
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> findSelector = null,
            string findTerm = null)
        {
            var data = FilterLogic(filter, orderBy, page, pageSize, findSelector, findTerm);

            return new PaginationEnvelope<T>
            {
                PageSize = pageSize ?? -1,
                PageNumber = page ?? -1,
                ItemCount = Count(filter, findSelector, findTerm),
                Data = data
            };
        }

        public T GetByKey(params object[] key)
        {
            return _dbSet.Find(key);
        }

        public T GetByKeyThrows(params object[] key)
        {
            var result = _dbSet.Find(key);
            if(result == null) throw new NotFoundException();
            return result;
        }

        public T Create()
        {
            var entity = _dbSet.Create<T>();
            return entity;
        }

        public T Insert(T entity)
        {
            var inDb = _dbSet.Add(entity);
            _context.SetState(inDb, EntityState.Added);
            return inDb;
        }

        public void DeleteByKey(params object[] key)
        {
            var entity = _dbSet.Find(key);
            Remove(entity);
        }

        public void DeleteBy(Expression<Func<T, bool>> selector)
        {
            var entity = _dbSet.SingleOrDefault(selector);
            Remove(entity);
        }

        public T Update(Action<T> updateFunction, bool throws = true, params object[] key)
        {
            var dbEntity = _dbSet.Find(key);
            return Update(updateFunction, dbEntity, throws);
        }

        public T Update(Action<T> updateFunction, params object[] key)
        {
            var dbEntity = _dbSet.Find(key);
            return Update(updateFunction, dbEntity);
        }

        public T UpdateBy(Action<T> updateFunction, Expression<Func<T, bool>> selector, bool throws = true)
        {
            var dbEntity = _dbSet.SingleOrDefault(selector);
            return Update(updateFunction, dbEntity, throws);
        }

        private T Update(Action<T> updateFunction, T entity, bool throws = true)
        {
            if (entity == null)
            {
                if(throws) throw new NotFoundException();
                return null;
            }
            updateFunction(entity);

            _context.SetState(entity, EntityState.Modified);

            return entity;
        }

        private IQueryable<T> FilterLogic(
            Expression<Func<T, bool>> filter, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            int? page, 
            int? pageSize,
            Expression<Func<T, string>> findSelector = null,
            string findTerm = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (findSelector != null && !string.IsNullOrWhiteSpace(findTerm))
                query = query.Similar(findSelector, findTerm);

            if (orderBy != null)
                query = orderBy(query);

            if (page.HasValue && pageSize.HasValue)
                query = query
                    .Skip((page.Value - 1)*pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }

        private IQueryable<T> FilterLogic(
            Expression<Func<T, bool>> filter,
            IEnumerable<string> orderBy,
            int? page,
            int? pageSize,
            Expression<Func<T, string>> findSelector = null,
            string findTerm = null)
        {
            IQueryable<T> query = _dbSet;
            
            if (filter != null)
                query = query.Where(filter);

            if (findSelector != null && !string.IsNullOrWhiteSpace(findTerm))
                query = query.Similar(findSelector, findTerm);

            if(orderBy == null) orderBy = new List<string>();
            var orderByString = string.Join(",", orderBy).Replace("_", " ");
            orderByString = string.IsNullOrEmpty(orderByString) ? "Id" : $"{orderByString},Id";
                query = query.OrderBy(orderByString);

            if (page.HasValue && pageSize.HasValue)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }

        private void Remove(T entity)
        {
            if (entity == null) return;

            _dbSet.Remove(entity);
        }
        
        public int Count(
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, string>> findSelector = null,
            string findTerm = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (findSelector != null && !string.IsNullOrWhiteSpace(findTerm))
                query = query.Similar(findSelector, findTerm);

            return query.Count();
        }
    }
}
