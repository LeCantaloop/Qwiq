using System;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    internal class QueryFactory : IQueryFactory
    {
        private readonly WorkItemTrackingHttpClient _store;

        private QueryFactory(WorkItemTrackingHttpClient store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public static IQueryFactory GetInstance(WorkItemTrackingHttpClient store)
        {
            return new QueryFactory(store);
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            // REVIEW: This can also throw an exception. Need to wrap in ExceptionHandlingDynamicProxyFactory call?
            var p = Parser.ParseSyntax(wiql);
            var w = new Wiql { Query = p.ToString() };

            return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(new QueryProxy(p, w, _store));
        }
    }
}