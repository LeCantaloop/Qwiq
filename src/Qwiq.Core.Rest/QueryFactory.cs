using System;
using System.Collections.Generic;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    internal class QueryFactory : IQueryFactory
    {
        private readonly WorkItemStore _store;

        private QueryFactory(WorkItemStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(
                new Query(new Wiql { Query = wiql }, dayPrecision, _store));
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IQuery>(
                new Query(ids, new Wiql { Query = wiql }, _store));
        }

        public IQuery Create(IEnumerable<int> ids, DateTime? asOf = null)
        {
            // The WIQL's WHERE and ORDER BY clauses are not used to filter (as we have specified IDs).
            // It is used for ASOF
            var wiql = $"SELECT {string.Join(", ", CoreFieldRefNames.All)} FROM WorkItems";
            if (asOf.HasValue)
            {
                // If specified DateTime is not UTC convert it to local time based on TFS client TimeZone
                if (asOf.Value.Kind != DateTimeKind.Utc)
                    asOf = DateTime.SpecifyKind(
                        asOf.Value - _store.TimeZone.GetUtcOffset(asOf.Value),
                        DateTimeKind.Utc);
                wiql += $" ASOF \'{asOf.Value:u}\'";
            }

            return Create(ids, wiql);
        }

        public static IQueryFactory GetInstance(WorkItemStore store)
        {
            return new QueryFactory(store);
        }
    }
}