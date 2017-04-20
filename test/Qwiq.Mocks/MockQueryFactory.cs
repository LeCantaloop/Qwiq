using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Qwiq.Mocks
{
    public class MockQueryFactory : IQueryFactory
    {
        private readonly MockWorkItemStore _store;

        private IList<string> _queries;

        public MockQueryFactory(MockWorkItemStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _queries = new List<string>();
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            CreateCallCount++;
            _queries.Add(wiql);
            return new MockQueryByWiql(wiql, _store);
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            CreateCallCount++;
            _queries.Add(wiql);
            return new MockQueryByWiql(ids, wiql, _store);
        }

        public IQuery Create(IEnumerable<int> ids, DateTime? asOf = null)
        {
            // The WIQL's WHERE and ORDER BY clauses are not used to filter (as we have specified IDs).
            // It is used for ASOF
            FormattableString ws = $"SELECT {string.Join(", ", CoreFieldRefNames.All)} FROM WorkItems";
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

        public int CreateCallCount { get; private set; }

        public IEnumerable<string> Queries => _queries;
    }
}
