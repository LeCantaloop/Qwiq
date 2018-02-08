using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Qwiq.Client.Rest
{
    internal class QueryFactory : IQueryFactory
    {
        [NotNull] private readonly WorkItemStore _store;

        private QueryFactory([NotNull] WorkItemStore store)
        {
            Contract.Requires(store != null);

            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            var q = new Query(new Wiql { Query = wiql }, dayPrecision, _store);
            return _store.Configuration.ProxyCreationEnabled
                ? q.AsProxy()
                : q;
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            var q = new Query(ids, new Wiql { Query = wiql }, _store);
            return _store.Configuration.ProxyCreationEnabled
                ? q.AsProxy()
                : q;
        }

        public IQuery Create(IEnumerable<int> ids, DateTime? asOf = null)
        {
            // The WIQL's WHERE and ORDER BY clauses are not used to filter (as we have specified IDs).
            // It is used for ASOF
            FormattableString ws = $"SELECT {string.Join(", ", _store.Configuration.DefaultFields ?? new[] { CoreFieldRefNames.Id })} FROM {WiqlConstants.WorkItemTable}";
            var wiql = ws.ToString(CultureInfo.InvariantCulture);

            if (asOf.HasValue)
            {
                // If specified DateTime is not UTC convert it to local time based on TFS client TimeZone
                if (asOf.Value.Kind != DateTimeKind.Utc)
                    asOf = DateTime.SpecifyKind(
                                                asOf.Value - _store.TimeZone.GetUtcOffset(asOf.Value),
                                                DateTimeKind.Utc);
                FormattableString ao = $" ASOF \'{asOf.Value:u}\'";
                wiql += ao.ToString(CultureInfo.InvariantCulture);
            }

            return Create(ids, wiql);
        }

        public static IQueryFactory GetInstance([NotNull] WorkItemStore store)
        {
            Contract.Requires(store != null);

            return new QueryFactory(store);
        }
    }
}