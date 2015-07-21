using System;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq.Linq.Tests.Mocks
{
    internal class MockWorkItemStore : IWorkItemStore
    {
        private readonly IEnumerable<IWorkItem> _workItems;
        private readonly IEnumerable<IWorkItemLinkInfo> _workItemLinks;

        public MockWorkItemStore(IEnumerable<IWorkItem> workItems, IEnumerable<IWorkItemLinkInfo> workItemLinks)
        {
            _workItems = workItems;
            _workItemLinks = workItemLinks;
        }

        public void Dispose()
        {
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            return _workItemLinks;
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            throw new NotImplementedException();
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            return _workItems;
        }

        public ITfsTeamProjectCollection TeamProjectCollection
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IProject> Projects
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes
        {
            get { throw new NotImplementedException(); }
        }
    }
}