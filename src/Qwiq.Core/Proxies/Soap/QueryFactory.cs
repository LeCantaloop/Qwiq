using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Soap;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class QueryFactory : IQueryFactory
    {
        private readonly WorkItemStore _store;

        private QueryFactory(WorkItemStore store)
        {
            _store = store;
        }

        public static IQueryFactory GetInstance(WorkItemStore store)
        {
            return new QueryFactory(store);
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(
                new QueryProxy(new Query(_store, wiql, null, dayPrecision)));
        }
    }
}