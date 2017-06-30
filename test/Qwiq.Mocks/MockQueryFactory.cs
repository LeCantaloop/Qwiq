using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mocks
{
    public class MockQueryFactory : IQueryFactory
    {
        [NotNull] private readonly MockWorkItemStore _store;

        public MockQueryFactory([NotNull] MockWorkItemStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IQuery Create(string wiql, bool dayPrecision = false)
        {
            return new MockQuery(_store, wiql);
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            return new MockQuery(_store, wiql, ids);
        }

        public IQuery Create(IEnumerable<int> ids, DateTime? asOf = null)
        {
            return new MockQuery(_store, null, ids);
        }
    }
}