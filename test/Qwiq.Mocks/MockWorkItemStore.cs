using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemStore : IWorkItemStore
    {
        private static readonly Random Instance = new Random();

        internal readonly IList<IWorkItemLinkInfo> LinkInfo;

        internal readonly IDictionary<int, IWorkItem> _lookup;

        private readonly Lazy<IFieldDefinitionCollection> _storeDefinitions;

        private Lazy<IProjectCollection> _projects;

        private Lazy<ITfsTeamProjectCollection> _tfs;

        private Lazy<IQueryFactory> _queryFactory;

        public MockWorkItemStore()
            : this(() => new MockTfsTeamProjectCollection(), store => new MockQueryFactory(store))
        {
        }

        [Obsolete("This method has been deprecated and will be removed in a future release.")]
        public MockWorkItemStore(IEnumerable<IWorkItem> workItems, IEnumerable<IWorkItemLinkInfo> links = null)
            : this()

        {
            this.Add(workItems, links);
        }

        public MockWorkItemStore(
            Func<ITfsTeamProjectCollection> tpcFactory,
            Func<MockWorkItemStore, IQueryFactory> queryFactory
            )
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));

            _tfs = new Lazy<ITfsTeamProjectCollection>(tpcFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory(this));
            _projects = new Lazy<IProjectCollection>(() => new MockProjectCollection(this));

            WorkItemLinkTypes = new WorkItemLinkTypeCollection(CoreLinkTypeReferenceNames.All.Select(s => new MockWorkItemLinkType(s)));
            _lookup = new Dictionary<int, IWorkItem>();
            LinkInfo = new List<IWorkItemLinkInfo>();
            _storeDefinitions = new Lazy<IFieldDefinitionCollection>(() => new MockFieldDefinitionCollection(this));
        }

        public bool SimulateQueryTimes { get; set; }

        private int WaitTime => Instance.Next(0, 3000);

        public TfsCredentials AuthorizedCredentials => null;

        public ClientType ClientType => ClientType.None;

        public IFieldDefinitionCollection FieldDefinitions => _storeDefinitions.Value;

        public IProjectCollection Projects => _projects.Value;

        public ITfsTeamProjectCollection TeamProjectCollection => _tfs.Value;

        public TimeZone TimeZone => _tfs.Value.TimeZone;

        public string UserAccountName => TeamProjectCollection.AuthorizedIdentity.GetUserAlias();

        public string UserDisplayName => TeamProjectCollection.AuthorizedIdentity.DisplayName;

        public string UserSid => TeamProjectCollection.AuthorizedIdentity.Descriptor.Identifier;

        public IWorkItemLinkTypeCollection WorkItemLinkTypes { get; internal set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            Trace.TraceInformation("Querying for work items " + wiql);

            var query = _queryFactory.Value.Create(wiql, dayPrecision);
            return query.RunQuery().ToList().AsReadOnly();
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var ids2 = (int[])ids.ToArray().Clone();
            if (!ids2.Any()) return Enumerable.Empty<IWorkItem>();



            Trace.TraceInformation("Querying for IDs " + string.Join(", ", ids2));

            var query = _queryFactory.Value.Create(ids2, asOf);
            return query.RunQuery().ToList().AsReadOnly();
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            return Query(new[] { id }, asOf).SingleOrDefault();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            Trace.TraceInformation("Querying for links " + wiql);
            var query = _queryFactory.Value.Create(wiql, dayPrecision);
            return query.RunLinkQuery().ToList().AsReadOnly();
        }

        public IRegisteredLinkTypeCollection RegisteredLinkTypes { get; }

        internal void BatchSave(params IWorkItem[] workItems)
        {
            BatchSave(workItems as IEnumerable<IWorkItem>);
        }

        internal void BatchSave(IEnumerable<IWorkItem> workItems)
        {
            var missingWits = new Dictionary<IProject, HashSet<IWorkItemType>>();

            foreach (var item in workItems)
            {

                Save(item);
            }

            foreach (var item in workItems)
            {
                var projectName = item[CoreFieldRefNames.TeamProject].ToString();
                var witName = item[CoreFieldRefNames.WorkItemType].ToString();
                var project = Projects[projectName];

                if (!project.WorkItemTypes.Contains(witName))
                {
                    Trace.TraceWarning("Project {0} is missing work item type definition {1}", project, witName);
                    missingWits.TryAdd(project, new HashSet<IWorkItemType>(WorkItemTypeComparer.Instance));

                    var t = item.Type as MockWorkItemType;
                    if (t?.Store != this) t.Store = this;

                    missingWits[project].Add(item.Type);
                }
            }

            if (!missingWits.Any()) return;
            var changesRequired = false;

            var newProjects = new List<IProject>();
            foreach (var project in Projects)
            {
                if (!missingWits.ContainsKey(project))
                {
                    newProjects.Add(project);
                    continue;
                }

                var wits = missingWits[project];

                if (!wits.Any())
                {
                    newProjects.Add(project);
                }
                else
                {
                    changesRequired = true;
                    wits.UnionWith(project.WorkItemTypes);
                    var w = new WorkItemTypeCollection(wits);
                    var p = new MockProject(
                        project.Guid,
                        project.Name,
                        project.Uri,
                        w,
                        project.AreaRootNodes,
                        project.IterationRootNodes);
                    newProjects.Add(p);
                }
            }

            if (changesRequired) _projects = new Lazy<IProjectCollection>(() => new MockProjectCollection(newProjects));
        }

        private void Save(IWorkItem item)
        {
            // Fix the ID
            if (item.Id == 0)
            {
                item[CoreFieldRefNames.Id] = _lookup.Keys.Any() ? _lookup.Keys.Max() + 1 : 1;
            }

            var id = item.Id;

            // Remove any existing links for this item
            //foreach (var link in _links.ToArray()) if (link.SourceId == id) _links.Remove(link);

            var l = new Dictionary<BaseLinkType, int>
                        {
                            { BaseLinkType.RelatedLink, 0 },
                            { BaseLinkType.ExternalLink, 0 },
                            { BaseLinkType.Hyperlink, 0 }
                        };

            // If there are new links add them back
            if (item.Links != null && item.Links.Any())
            {
                foreach (var link in item.Links)
                {
                    l[link.BaseType]++;

                    // We only support related links at the moment
                    if (link.BaseType != BaseLinkType.RelatedLink) continue;
                    var rl = link as IRelatedLink;
                    if (rl == null) continue;

                    var mrl = rl as MockRelatedLink;
                    if (mrl != null)
                    {
                        var li = mrl.LinkInfo;
                        if (LinkInfo.Contains(li, WorkItemLinkInfoComparer.Instance))
                        {
                            Trace.TraceWarning(
                                               $"Warning: Duplicate link. (Type: {li.LinkType?.ImmutableName ?? "NULL"}; Source: {li.SourceId}; Target: {li.TargetId})");
                        }
                        else
                        {
                            LinkInfo.Add(li);
                        }

                        if (rl.LinkTypeEnd == null) continue;

                        // Check to see if a recipricol link is required
                        if (rl.LinkTypeEnd.LinkType.IsDirectional)
                        {
                            var t = _lookup[rl.RelatedWorkItemId];
                            var e = rl.LinkTypeEnd.OppositeEnd;

                            // Check to see if an existing link exists
                            if (!t.Links.OfType<IRelatedLink>().Any(p => p.RelatedWorkItemId == id && Equals(p.LinkTypeEnd, e)))
                            {
                                // There is not--create one
                                var tl = t.CreateRelatedLink(id, rl.LinkTypeEnd.OppositeEnd);
                                t.Links.Add(tl);
                                Save(t);
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            item[CoreFieldRefNames.RelatedLinkCount] = l[BaseLinkType.RelatedLink];
            item[CoreFieldRefNames.ExternalLinkCount] = l[BaseLinkType.ExternalLink];
            item[CoreFieldRefNames.HyperLinkCount] = l[BaseLinkType.Hyperlink];

            // Fix up Team Project if needed
            var projectName = item[CoreFieldRefNames.TeamProject]?.ToString();
            if (string.IsNullOrEmpty(projectName))
            {
                projectName = Projects[0].Name;
                item[CoreFieldRefNames.TeamProject] = projectName;
            }
            _lookup[id] = item;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}