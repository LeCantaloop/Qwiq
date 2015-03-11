using Microsoft.IE.Qwiq.Proxies;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    internal interface IQueryFactory
    {
        IQuery Create(string wiql, bool dayPrecision);
    }

    internal class QueryFactory : IQueryFactory
    {
        private readonly Tfs.WorkItemStore _store;

        private QueryFactory(Tfs.WorkItemStore store)
        {
            _store = store;
        }

        public static QueryFactory GetInstance(Tfs.WorkItemStore store)
        {
            return new QueryFactory(store);
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            return new QueryProxy(new Tfs.Query(_store, wiql, null, dayPrecision));
        }
    }
}
