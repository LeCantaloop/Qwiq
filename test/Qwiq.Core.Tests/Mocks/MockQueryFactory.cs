using System.Collections.Generic;

namespace Microsoft.Qwiq.Core.Tests.Mocks
{
    public class MockQueryFactory : IQueryFactory
    {
        private readonly List<string> _wiqls = new List<string>();
        public IQuery Create(string wiql, bool dayPrecision)
        {
            CreateCallCount++;
            return new MockQuery(wiql, dayPrecision, _wiqls);
        }

        public IQuery Create(IEnumerable<int> ids, string wiql)
        {
            CreateCallCount++;
            return new MockQuery(wiql, false, _wiqls);
        }

        public int CreateCallCount { get; private set; }

        public IEnumerable<string> QueryWiqls => _wiqls;
    }
}
