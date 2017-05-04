using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Common;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class QueryFactory : IQueryFactory
    {
        [NotNull]
        private readonly WorkItemStore _store;

        internal QueryFactory([NotNull] WorkItemStore store)
        {
            Contract.Requires(store != null);

            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            var q = new Query(
                              new TeamFoundation.WorkItemTracking.Client.Query(_store.NativeWorkItemStore, wiql, null, dayPrecision),
                              _store.Configuration.PageSize);

            return _store.Configuration.ProxyCreationEnabled
                ? ExceptionHandlingDynamicProxyFactory.Create<IQuery>(q)
                : q;
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            if (string.IsNullOrWhiteSpace(wiql))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(wiql));

            var q = new Query(
                              new TeamFoundation.WorkItemTracking.Client.Query(_store.NativeWorkItemStore, wiql, ids.ToArray()),
                              _store.Configuration.PageSize);

            return _store.Configuration.ProxyCreationEnabled
                       ? ExceptionHandlingDynamicProxyFactory.Create<IQuery>(q)
                       : q;
        }

        public IQuery Create(IEnumerable<int> ids, DateTime? asOf = null)
        {
            // The WIQL's WHERE and ORDER BY clauses are not used to filter (as we have specified IDs).
            // It is used for ASOF
            var wiql =
                $"SELECT {string.Join(", ", CoreFieldRefNames.All.Select(WiqlHelpers.GetEnclosedField))} FROM {WiqlConstants.WorkItemTable}";
            if (asOf.HasValue) wiql += $" {QueryPackageNames.QueryAsOf} \'{asOf.Value:u}\'";
            return Create(ids, wiql);
        }
    }
}