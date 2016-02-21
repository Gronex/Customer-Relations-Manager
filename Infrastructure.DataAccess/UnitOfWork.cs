using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainServices;
using Infrastructure.DataAccess.Exceptions;

namespace Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public static int SqlDuplicateKeyException { get; } = 2601;

        private readonly DbContext _context;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
        }

        public int Save()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx) // verbose failure message
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
                throw;
            }
            catch (DbUpdateException ex) when ((ex.InnerException?.InnerException as SqlException)?.Number == SqlDuplicateKeyException)
            {
                //Should not be able to be null since the when should be able to catch it
                if (ex.InnerException != null)
                    throw new DuplicateException(ex.InnerException.InnerException.Message);
                throw;
            }
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx) // verbose failure message
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }

        #region IDisposable

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
