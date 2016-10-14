using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Relatives.Linq
{
    public static class QueryExtensions
    {
        public static IEnumerable<KeyValuePair<TParent, IEnumerable<TChild>>> Parents<TParent, TChild>(this IQueryable<TChild> query)
            //where TParent : IWorkItem
            //where TChild : IWorkItem
        {
            return query.Select(item => item.Parents<TParent, TChild>());
        }

        private static KeyValuePair<TParent, IEnumerable<TChild>> Parents<TParent, TChild>(this TChild _)
        {
            return new KeyValuePair<TParent, IEnumerable<TChild>>();
        }

        public static IEnumerable<KeyValuePair<TParent, IEnumerable<TChild>>> Children<TParent, TChild>(this IQueryable<TParent> query)
            //where TParent : IWorkItem
            //where TChild : IWorkItem
        {
            return query.Select(item => item.Children<TParent, TChild>());
        }

        private static KeyValuePair<TParent, IEnumerable<TChild>> Children<TParent, TChild>(this TParent _)
        {
            return new KeyValuePair<TParent, IEnumerable<TChild>>();
        }
    }
}

