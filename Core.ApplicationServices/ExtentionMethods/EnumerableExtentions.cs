using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices.ExtentionMethods
{
    public static class EnumerableExtentions
    {
        public static IEnumerable<TSource> WhereWithLookahead<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> predicate) where TSource : class
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    //empty
                    yield break;
                }

                var current = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    var next = enumerator.Current;

                    if (predicate(current, next))
                    {
                        yield return current;
                    }

                    current = next;
                }

                if (predicate(current, null))
                {
                    yield return current;
                }

            }
        }
    }
}
