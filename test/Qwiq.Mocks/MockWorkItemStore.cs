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

        private readonly Lazy<IFieldDefinitionCollection> _storeDefinitions;

        private readonly IDictionary<int, IWorkItem> _lookup;

        private readonly IList<IWorkItem> _workItems;

        public MockWorkItemStore()
            : this(new MockTfsTeamProjectCollection())
        {
        }

        public MockWorkItemStore(ITfsTeamProjectCollection teamProjectCollection)
            : this(teamProjectCollection, new MockProject())
        {
        }

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See ctor(ITfsTeamProjectCollection, IProject, IEnumerable<IWorkItemLinkType>, IEnumerable<IWorkItem>, IEnumerable<IWorkItemLinkInfo>).")]
        public MockWorkItemStore(IEnumerable<IWorkItem> workItems, IEnumerable<IWorkItemLinkInfo> links = null)
            : this(new MockTfsTeamProjectCollection(), new MockProject(), null, workItems, links)
        {
        }

        public MockWorkItemStore(
            ITfsTeamProjectCollection teamProjectCollection,
            IProject project,
            IEnumerable<IWorkItemLinkType> linkTypes = null,
            IEnumerable<IWorkItem> workItems = null,
            IEnumerable<IWorkItemLinkInfo> links = null)
        {
            TeamProjectCollection = teamProjectCollection
                                    ?? throw new ArgumentNullException(nameof(teamProjectCollection));
            if (project == null) throw new ArgumentNullException(nameof(project));
            TimeZone = teamProjectCollection.TimeZone;

            WorkItemLinkTypes = new WorkItemLinkTypeCollection(
                linkTypes ?? CoreLinkTypeReferenceNames.All.Select(s => new MockWorkItemLinkType(s)));

            if (workItems == null)
            {
                Projects = new[] { project };
                _workItems = new List<IWorkItem>();

                foreach (var wit in project.WorkItemTypes)
                {
                    var wi = new MockWorkItem(wit) { Id = _workItems.Count + 1 };
                    _workItems.Add(wi);
                }
            }
            else
            {
                _workItems = workItems.ToList();

                var missing = _workItems
                    .Select(s => s.Type)
                    .Distinct(WorkItemTypeComparer.Instance)
                    .Where(wit => !project.WorkItemTypes.Contains(wit, WorkItemTypeComparer.Instance))
                    .ToList();

                if (missing.Any())
                {
                    Trace.TraceWarning(
                        "Project {0} is missing the following work item type definitions: {1}.",
                        project.Name,
                        string.Join(", ", missing));
                    // Add the missing WITs to the incoming project
                    project = new MockProject(project.WorkItemTypes.Union(missing).Distinct());
                }

                Projects = new[] { project };
            }

            _lookup = _workItems.ToDictionary(k => k.Id, e => e);
            _links = links?.ToList() ?? Enumerable.Empty<IWorkItemLinkInfo>();
            _storeDefinitions = new Lazy<IFieldDefinitionCollection>(() => new MockFieldDefinitionCollection(this));
        }

        public bool SimulateQueryTimes { get; set; }

        private int WaitTime => Instance.Next(0, 3000);

        public TfsCredentials AuthorizedCredentials => null;

        public IFieldDefinitionCollection FieldDefinitions => _storeDefinitions.Value;

        public IEnumerable<IProject> Projects { get; }

        public ITfsTeamProjectCollection TeamProjectCollection { get; }

        public TimeZone TimeZone { get; }

        public string UserDisplayName => TeamProjectCollection.AuthorizedIdentity.DisplayName;

        public string UserIdentityName => TeamProjectCollection.AuthorizedIdentity.DisplayName;

        public string UserSid => TeamProjectCollection.AuthorizedIdentity.Descriptor.Identifier;

        public WorkItemLinkTypeCollection WorkItemLinkTypes { get; }

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
    }
}