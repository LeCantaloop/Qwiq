using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemStore : IWorkItemStore
    {
        internal readonly IDictionary<int, IWorkItem> _lookup;
        internal readonly IList<IWorkItemLinkInfo> LinkInfo;
        private static readonly Random Instance = new Random();
        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IFieldDefinitionCollection> _storeDefinitions;

        private readonly Lazy<ITeamProjectCollection> _tfs;

        private Lazy<IProjectCollection> _projects;

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

        public MockWorkItemStore(Func<ITeamProjectCollection> tpcFactory, Func<MockWorkItemStore, IQueryFactory> queryFactory)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));

            _tfs = new Lazy<ITeamProjectCollection>(tpcFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory(this));
            _projects = new Lazy<IProjectCollection>(() => new MockProjectCollection(this));

            WorkItemLinkTypes = new WorkItemLinkTypeCollection(
                                                               CoreLinkTypeReferenceNames
                                                                       .All.Select(s => (IWorkItemLinkType)new MockWorkItemLinkType(s))
                                                                       .ToList());
            _lookup = new Dictionary<int, IWorkItem>();
            LinkInfo = new List<IWorkItemLinkInfo>();
            _storeDefinitions = new Lazy<IFieldDefinitionCollection>(() => new MockFieldDefinitionCollection(this));

            Configuration = new MockWorkItemStoreConfiguration();
        }

        public VssCredentials AuthorizedCredentials => null;

        /// <inheritdoc/>
        ///
        public WorkItemStoreConfiguration Configuration { get; }

        public IFieldDefinitionCollection FieldDefinitions => _storeDefinitions.Value;
        public IProjectCollection Projects => _projects.Value;
        public IRegisteredLinkTypeCollection RegisteredLinkTypes { get; }
        public bool SimulateQueryTimes { get; set; }
        public ITeamProjectCollection TeamProjectCollection => _tfs.Value;
        public IWorkItemLinkTypeCollection WorkItemLinkTypes { get; internal set; }
        public ITeamFoundationIdentity AuthorizedIdentity => TeamProjectCollection.AuthorizedIdentity;
        public TimeZone TimeZone => _tfs.Value.TimeZone;
        private int WaitTime => Instance.Next(0, 3000);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IWorkItemCollection Query(string wiql, bool dayPrecision = false)
        {
            Trace.TraceInformation("Querying for work items " + wiql);

            var query = _queryFactory.Value.Create(wiql, dayPrecision);
            return query.RunQuery();
        }

        public IWorkItemCollection Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var ids2 = (int[])ids.ToArray().Clone();
            if (!ids2.Any()) return Enumerable.Empty<IWorkItem>().ToWorkItemCollection();

            Trace.TraceInformation("Querying for IDs " + string.Join(", ", ids2));

            var query = _queryFactory.Value.Create(ids2, asOf);
            return query.RunQuery();
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

        internal void BatchSave(IEnumerable<IWorkItem> workItems)
        {
            // First: Fix up the work items and save them to our dictionary

            foreach (var item in workItems) Save(item);

            // Second: Update the links for the work items We need to save first so we can create recipricol links if required (e.g. parent -> child also needs a child -> parent)
            foreach (var item in workItems) SaveLinks(item);

            // Third: If any of the work items have types that are missing from their project, add those
            var missingWits = new Dictionary<IProject, HashSet<IWorkItemType>>();
            foreach (var item in workItems)
            {
                var projectName = item[CoreFieldRefNames.TeamProject].ToString();
                var witName = item[CoreFieldRefNames.WorkItemType].ToString();
                IProject project;
                try
                {
                    project = Projects[projectName];
                }
                catch (DeniedOrNotExistException)
                {
                    Trace.TraceWarning("Project {0} missing from work item store.", projectName);
                    project = new MockProject(this);
                }

                if (!project.WorkItemTypes.Contains(witName))
                {
                    Trace.TraceWarning("Project {0} is missing work item type definition {1}", project, witName);
                    missingWits.TryAdd(project, new HashSet<IWorkItemType>(WorkItemTypeComparer.Default));

                    var t = item.Type as MockWorkItemType;
                    if (t?.Store != this) t.Store = this;

                    missingWits[project].Add(item.Type);
                }
            }

            // Fourth: If there are any missing wits update the project and reset the project collection
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
                    var w = new WorkItemTypeCollection(wits.ToList());
                    var p = new MockProject(project.Guid, project.Name, project.Uri, w, project.AreaRootNodes, project.IterationRootNodes);
                    newProjects.Add(p);
                }
            }

            if (changesRequired) _projects = new Lazy<IProjectCollection>(() => new MockProjectCollection(newProjects));
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        private void Save(IWorkItem item)
        {
            // Fix the ID
            if (item.Id == 0) item[CoreFieldRefNames.Id] = _lookup.Keys.Any() ? _lookup.Keys.Max() + 1 : 1;

            var id = item.Id;

            var l = new Dictionary<BaseLinkType, int>
                        {
                            { BaseLinkType.RelatedLink, 0 },
                            { BaseLinkType.ExternalLink, 0 },
                            { BaseLinkType.Hyperlink, 0 }
                        };

            if (item.Links != null && item.Links.Any()) foreach (var link in item.Links) l[link.BaseType]++;

            item[CoreFieldRefNames.RelatedLinkCount] = l[BaseLinkType.RelatedLink];
            item[CoreFieldRefNames.ExternalLinkCount] = l[BaseLinkType.ExternalLink];
            item[CoreFieldRefNames.HyperlinkCount] = l[BaseLinkType.Hyperlink];

            // Fix up Team Project if needed
            var projectName = item[CoreFieldRefNames.TeamProject]?.ToString();
            if (string.IsNullOrEmpty(projectName))
            {
                projectName = Projects[0].Name;
                item[CoreFieldRefNames.TeamProject] = projectName;
            }

            // Fix up Store if needed
            if (item.Type is MockWorkItemType t)
            {
                if (t.Store == null || t.Store != this)
                {
                    t.Store = this;
                }
            }

            _lookup[id] = item;
        }

        private void SaveLink(ILink link, int id)
        {
            // We only support related links at the moment
            if (link.BaseType != BaseLinkType.RelatedLink) return;
            var rl = link as IRelatedLink;
            if (rl == null) return;

            if (rl is MockRelatedLink mrl)
            {
                var li = mrl.LinkInfo;
                if (LinkInfo.Contains(li, WorkItemLinkInfoComparer.Default))
                    Trace.TraceWarning(
                                       $"Warning: Duplicate link. (Type: {li.LinkType?.ImmutableName ?? "NULL"}; Source: {li.SourceId}; Target: {li.TargetId})");
                else LinkInfo.Add(li);

                if (rl.LinkTypeEnd == null) return;

                // Check to see if a recipricol link is required
                if (rl.LinkTypeEnd.LinkType.IsDirectional)
                    try
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
                    catch (KeyNotFoundException)
                    {
                        Trace.TraceWarning(
                                           $"Work item {id} contains a {rl.LinkTypeEnd} to an item that does not exist: {rl.RelatedWorkItemId}.");
                    }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private void SaveLinks(IWorkItem item)
        {
            var id = item.Id;
            // If there are new links add them back
            if (item.Links != null && item.Links.Any()) foreach (var link in item.Links) SaveLink(link, id);
        }
    }
}