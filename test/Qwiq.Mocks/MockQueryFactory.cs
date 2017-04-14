using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mocks
{
    public class MockQueryFactory : IQueryFactory
    {
        private readonly MockWorkItemStore _store;

        private readonly List<string> _wiqls = new List<string>();

        public MockQueryFactory(MockWorkItemStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IQuery Create(string wiql, bool dayPrecision)
        {
            CreateCallCount++;
            _wiqls.Add(wiql);
            return new MockQueryByWiql(wiql, _store);
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            CreateCallCount++;
            _wiqls.Add(wiql);
            return new MockQueryByWiql(ids, wiql, _store);
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

        public int CreateCallCount { get; private set; }

        public IEnumerable<string> QueryWiqls => _wiqls;
    }
}
