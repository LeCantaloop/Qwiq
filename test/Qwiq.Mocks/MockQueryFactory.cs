using System;
using System.Collections.Generic;

namespace Qwiq.Mocks
{
    public class MockQueryFactory : IQueryFactory
    {
        private readonly MockWorkItemStore _store;

        public MockQueryFactory(MockWorkItemStore store)
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