using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace Qwiq.Mocks
{
    public class MockQueryByWiqlFactory : IQueryFactory
    {
        [NotNull] private readonly MockWorkItemStore _store;

        public MockQueryByWiqlFactory([NotNull] MockWorkItemStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            return new MockQueryByWiql(wiql, _store);
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            return new MockQueryByWiql(ids, wiql, _store);
        }

        public IQuery Create(IEnumerable<int> ids, DateTime? asOf = null)
        {
            // The WIQL's WHERE and ORDER BY clauses are not used to filter (as we have specified IDs).
            // It is used for ASOF
            FormattableString ws = $"SELECT {string.Join(", ", CoreFieldRefNames.All)} FROM {WiqlConstants.WorkItemTable}";
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
    }
}