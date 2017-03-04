using System;
using System.Linq;

namespace Microsoft.Qwiq.Linq
{
    public static class QueryExtensions
    {
        public static IQueryable<T> AsOf<T>(this IQueryable<T> query, DateTime asOfDateTime)
        {
            return query.Where(workItem => workItem.AsOf(asOfDateTime));
        }

        // We need to define a private version that operates on the thing being queried. If tried to use the public AsOf
        // directly, LINQ would automatically inline it to its consitutent Where call, which means we wouldn't have any
        // opportunity to detect it. With this private extension method now the query provider sees the AsOf, allowing
        // us to hook it.
        private static bool AsOf<T>(this T _, DateTime __)
        {
            return true;
        }

        public static bool WasEver<T>(this T _, T __)
        {
            return true;
        }
    }
}

