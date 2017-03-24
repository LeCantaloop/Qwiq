using System;
using System.Collections.Generic;
using System.Linq;

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

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            if (wiql == null) throw new ArgumentNullException(nameof(wiql));

            return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(new QueryProxy(new Query(_store, wiql, ids.ToArray())));
        }
    }
}