using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Core.DomainServices;

namespace Infrastructure.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? page = null,
            int? pageSize = null)
        {
            return FilterLogic(filter, orderBy, page, pageSize);
        }

        public T GetByKey(params object[] key)
        {
            return _dbSet.Find(key);
        }

        public T Create()
        {
            var entity = _dbSet.Create<T>();
            return entity;
        }

        public T Insert(T entity)
        {
            return _dbSet.Add(entity);
        }

        public void DeleteByKey(params object[] key)
        {
            var entity = _dbSet.Find(key);
            if (entity == null) return;

            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
        }
        
        public T Update(Func<T, T> updateFunction, params object[] key)
        {
            var dbEntity = _dbSet.Find(key);
            if (dbEntity == null) return null;

            var updated = updateFunction(dbEntity);

            foreach(var prop in typeof(T).GetProperties().Where(p => p.Name != "Id"))
            {
                _context.Entry(updated).Property(prop.Name).IsModified = true;
            }

            return updated;
        }

        private IQueryable<T> FilterLogic(
            Expression<Func<T, bool>> filter, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            int? page, 
            int? pageSize)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page.HasValue && pageSize.HasValue)
                query = query
                    .Skip((page.Value - 1)*pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }
    }
}
