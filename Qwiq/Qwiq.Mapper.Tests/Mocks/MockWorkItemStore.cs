using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    class MockWorkItemStore : IWorkItemStore
    {
        private readonly IEnumerable<IWorkItem> _workItems;

        public MockWorkItemStore(IEnumerable<IWorkItem> workItems)
        {
            _workItems = workItems;
        }

        public void Dispose()
        {
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            return _workItems.Where(item => ids.Contains(item.Id));
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            throw new NotImplementedException();
        }

        public ITfsTeamProjectCollection TeamProjectCollection { get; private set; }
        public IEnumerable<IProject> Projects { get; private set; }
        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes { get; private set; }
    }
}