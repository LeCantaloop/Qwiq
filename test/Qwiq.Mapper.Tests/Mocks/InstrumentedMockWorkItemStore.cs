using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    internal class InstrumentedMockWorkItemStore : IWorkItemStore
    {
        private readonly IWorkItemStore _innerWorkItemStore;

        public InstrumentedMockWorkItemStore(IWorkItemStore innerWorkItemStore)
        {
            _innerWorkItemStore = innerWorkItemStore;
        }

        public IEnumerable<IProject> Projects
        {
            get
            {
                ProjectsCallCount += 1;
                return _innerWorkItemStore.Projects;
            }
        }

        public int ProjectsCallCount { get; private set; }

        public int QueryCallCount
        {
            get
            {
                return QueryIdCallCount + QueryIdsCallCount + QueryLinksCallCount + QueryStringCallCount;
            }
        }

        public int QueryIdCallCount { get; private set; }

        public int QueryIdsCallCount { get; private set; }

        public int QueryLinksCallCount { get; private set; }

        public int QueryStringCallCount { get; private set; }

        public ITfsTeamProjectCollection TeamProjectCollection
        {
            get
            {
                TeamProjectCollectionCallCount += 1;
                return _innerWorkItemStore.TeamProjectCollection;
            }
        }

        public int TeamProjectCollectionCallCount { get; private set; }

        public TimeZone TimeZone => _innerWorkItemStore.TimeZone;

        public string UserDisplayName => _innerWorkItemStore.UserDisplayName;

        public string UserIdentityName => _innerWorkItemStore.UserIdentityName;

        public string UserSid => _innerWorkItemStore.UserSid;

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes
        {
            get
            {
                WorkItemLinkTypesCallCount += 1;
                return _innerWorkItemStore.WorkItemLinkTypes;
            }
        }

        public int WorkItemLinkTypesCallCount { get; private set; }

        public void Dispose()
        {
            _innerWorkItemStore.Dispose();
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            QueryStringCallCount += 1;
            return _innerWorkItemStore.Query(wiql, dayPrecision);
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            QueryIdsCallCount += 1;
            return _innerWorkItemStore.Query(ids, asOf);
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            QueryIdCallCount += 1;
            return _innerWorkItemStore.Query(id, asOf);
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            QueryLinksCallCount += 1;
            return _innerWorkItemStore.QueryLinks(wiql, dayPrecision);
        }
    }
}