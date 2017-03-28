using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemStore : IWorkItemStore
    {
        private static readonly Random Instance = new Random();

        private readonly IEnumerable<IWorkItemLinkInfo> _links;

        private IDictionary<int, IWorkItem> _lookup;

        private IList<IWorkItem> _workItems;

        [Obsolete("This method has been deprecated and will be removed in a future release. See ctor(ITfsTeamProjectCollection).")]
        public MockWorkItemStore()
            : this(new MockTfsTeamProjectCollection())
        {
        }

        public MockWorkItemStore(ITfsTeamProjectCollection teamProjectCollection)
        {
            TeamProjectCollection = teamProjectCollection
                                    ?? throw new ArgumentNullException(nameof(teamProjectCollection));

            TimeZone = teamProjectCollection.TimeZone;

            var project = new MockProject(this);

            InitializeProject(project);
        }

        public MockWorkItemStore(ITfsTeamProjectCollection teamProjectCollection, IProject project)
            : this(teamProjectCollection)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            InitializeProject(project);
        }

        [Obsolete("This method has been deprecated and will be removed in a future release. See ctor(ITfsTeamProjectCollection, IEnumerable<IWorkItem>).")]
        public MockWorkItemStore(IEnumerable<IWorkItem> workItems)
            : this()
        {
            _workItems = new List<IWorkItem>(workItems);
            _lookup = _workItems.ToDictionary(k => k.Id, e => e);
        }

        [Obsolete("This method has been deprecated and will be removed in a future release. See ctor(ITfsTeamProjectCollection, IEnumerable<IWorkItem>).")]
        public MockWorkItemStore(IEnumerable<IWorkItem> workItems, IEnumerable<IWorkItemLinkInfo> links)
            : this(workItems)
        {
            _links = links;
        }

        public MockWorkItemStore(ITfsTeamProjectCollection teamProjectCollection, IEnumerable<IWorkItem> workItems)
            : this(teamProjectCollection)
        {
            _workItems = new List<IWorkItem>(workItems);
            _lookup = _workItems.ToDictionary(k => k.Id, e => e);
        }

        public bool SimulateQueryTimes { get; set; }

        private int WaitTime => Instance.Next(0, 3000);

        public TfsCredentials AuthorizedCredentials => null;

        public IFieldDefinitionCollection FieldDefinitions => new MockFieldDefinitionCollection(Projects.SelectMany(s => s.WorkItemTypes).SelectMany(s => s.FieldDefinitions).Select(s => s));

        public IEnumerable<IProject> Projects { get; set; }

        public ITfsTeamProjectCollection TeamProjectCollection { get; set; }

        public TimeZone TimeZone { get; }

        public string UserDisplayName => TeamProjectCollection.AuthorizedIdentity.DisplayName;

        public string UserIdentityName => TeamProjectCollection.AuthorizedIdentity.DisplayName;

        public string UserSid => TeamProjectCollection.AuthorizedIdentity.Descriptor.Identifier;



        public WorkItemLinkTypeCollection WorkItemLinkTypes
        {
            get
            {
                return new WorkItemLinkTypeCollection(CoreLinkTypeReferenceNames.All.Select(s => new MockWorkItemLinkType(s)));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            if (_workItems == null)
                throw new InvalidOperationException("Class must be initialized with set of IWorkItems.");

            Trace.TraceInformation("Querying for work items " + wiql);

            if (SimulateQueryTimes)
            {
                var sleep = WaitTime;
                Trace.TraceInformation("Sleeping thread {0} ms to simulate query time", WaitTime);
                Thread.Sleep(sleep);
            }

            return _workItems;
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (_workItems == null)
                throw new InvalidOperationException("Class must be initialized with set of IWorkItems.");

            var h = new HashSet<int>(ids);

            Trace.TraceInformation("Querying for IDs " + string.Join(", ", h));

            if (SimulateQueryTimes)
            {
                var sleep = WaitTime;
                Trace.TraceInformation("Sleeping thread {0} ms to simulate query time", WaitTime);
                Thread.Sleep(sleep);
            }

            return _lookup.Where(p => h.Contains(p.Key)).Select(p => p.Value).ToList();
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            return Query(new[] { id }).SingleOrDefault();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            Trace.TraceInformation("Querying for links " + wiql);
            if (SimulateQueryTimes)
            {
                var sleep = WaitTime;
                Trace.TraceInformation("Sleeping thread {0} ms to simulate query time", WaitTime);
                Thread.Sleep(sleep);
            }
            return _links;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        private void InitializeProject(IProject project)
        {
            Projects = new[] { project };

            _workItems = new List<IWorkItem>();

            foreach (var wit in project.WorkItemTypes)
            {
                var wi = new MockWorkItem(wit) { Id = _workItems.Count + 1, Type = wit };
                _workItems.Add(wi);
            }

            _lookup = _workItems.ToDictionary(k => k.Id, e => e);
        }
    }
}