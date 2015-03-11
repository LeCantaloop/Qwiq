using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.UnitTests.Mocks
{
    public class MockQuery : IQuery
    {
        public MockQuery(string wiql, bool dayPrecision, IList<string> wiqls)
        {
            wiqls.Add(wiql);
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            return Enumerable.Empty<IWorkItem>();
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            return Enumerable.Empty<IWorkItemLinkInfo>();
        }
    }
}