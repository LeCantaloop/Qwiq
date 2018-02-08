using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Services.Common;

namespace Qwiq.Mapper.Mocks
{
    internal class InstrumentedMockWorkItemStore : IWorkItemStore
    {
        private readonly IWorkItemStore _innerWorkItemStore;

        public InstrumentedMockWorkItemStore(IWorkItemStore innerWorkItemStore)
        {
            _innerWorkItemStore = innerWorkItemStore;
        }

        public VssCredentials AuthorizedCredentials => _innerWorkItemStore.AuthorizedCredentials;

        public ITeamFoundationIdentity AuthorizedIdentity => _innerWorkItemStore?.AuthorizedIdentity;

        /// <inheritdoc />
        public WorkItemStoreConfiguration Configuration => _innerWorkItemStore.Configuration;

        public IFieldDefinitionCollection FieldDefinitions => _innerWorkItemStore.FieldDefinitions;

        public IProjectCollection Projects
        {
            get
            {
                ProjectsCallCount += 1;
                return _innerWorkItemStore.Projects;
            }
        }

        public int ProjectsCallCount { get; private set; }

        public int QueryCallCount => QueryIdCallCount + QueryIdsCallCount + QueryLinksCallCount + QueryStringCallCount;

        public int QueryIdCallCount { get; private set; }

        public int QueryIdsCallCount { get; private set; }

        public int QueryLinksCallCount { get; private set; }

        public int QueryStringCallCount { get; private set; }

        public IRegisteredLinkTypeCollection RegisteredLinkTypes => _innerWorkItemStore.RegisteredLinkTypes;

        public ITeamProjectCollection TeamProjectCollection
        {
            get
            {
                TeamProjectCollectionCallCount += 1;
                return _innerWorkItemStore.TeamProjectCollection;
            }
        }

        public int TeamProjectCollectionCallCount { get; private set; }

        public TimeZone TimeZone => _innerWorkItemStore.TimeZone;

        public IWorkItemLinkTypeCollection WorkItemLinkTypes
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

        public IWorkItemCollection Query(string wiql, bool dayPrecision = false)
        {
            QueryStringCallCount += 1;
            return _innerWorkItemStore.Query(wiql, dayPrecision);
        }

        public IWorkItemCollection Query(IEnumerable<int> ids, DateTime? asOf = null)
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