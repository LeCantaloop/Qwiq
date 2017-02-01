using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies;
using Microsoft.Qwiq.Proxies.Soap;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq
{
    internal interface IQueryFactory
    {
        IQuery Create(string wiql, bool dayPrecision);
    }

    namespace Microsoft.Qwiq.Soap
    {
        internal class QueryFactory : IQueryFactory
        {
            private readonly Tfs.WorkItemStore _store;

            private QueryFactory(Tfs.WorkItemStore store)
            {
                _store = store;
            }

            public static IQueryFactory GetInstance(Tfs.WorkItemStore store)
            {
                return new QueryFactory(store);
            }

            public IQuery Create(string wiql, bool dayPrecision)
            {
                return
                    ExceptionHandlingDynamicProxyFactory.Create<IQuery>(
                        new QueryProxy(new Tfs.Query(_store, wiql, null, dayPrecision)));
            }
        }
    }

    namespace Microsoft.Qwiq.Rest
    {
        internal class QueryFactory : IQueryFactory
        {
            private readonly WorkItemTrackingHttpClient _store;

            private QueryFactory(WorkItemTrackingHttpClient store)
            {
                _store = store;
            }

            public static IQueryFactory GetInstance(WorkItemTrackingHttpClient store)
            {
                return new QueryFactory(store);
            }

            public IQuery Create(string wiql, bool dayPrecision)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

