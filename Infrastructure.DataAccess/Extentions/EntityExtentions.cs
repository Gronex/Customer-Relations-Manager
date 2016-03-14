using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DataAccess.Exceptions;

namespace Infrastructure.DataAccess.Extentions
{
    public static class EntityExtentions
    {
        public static T SingleOrExcept<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
        {
            var result = query.SingleOrDefault(predicate);
            if (result == null) throw new NotFoundException();
            return result;
        }
    }
}
