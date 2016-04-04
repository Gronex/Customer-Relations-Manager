using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
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

        public static IQueryable<T> Similar<T>(this IQueryable<T> list, Expression<Func<T, string>> selector, string term)
        {
            var stripedTerm = term.ToLower().RemoveSeperators();
            return list
                .GroupBy(selector)
                .Where(x => x.Key.ToLower().Replace(" ", "").Replace("-", "").Replace("_", "").Contains(stripedTerm))
                .Select(x => x.FirstOrDefault());
        }

        private static string RemoveSeperators(this string input)
        {
            return input.Replace(" ", "").Replace("-", "").Replace("_", "");
        }

        public static void ReplaceCollection<T>(this ICollection<T> collection, IEnumerable<T> updatedCollection)
        {
            collection.Clear();
            foreach (var item in updatedCollection)
            {
                collection.Add(item);
            }
        }
    }
}
