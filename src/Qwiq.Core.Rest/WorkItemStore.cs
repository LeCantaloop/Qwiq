using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Qwiq.Client.Rest
{
    internal class WorkItemStore : IWorkItemStore
    {
        private static readonly Regex ImmutableLinkTypeNameRegex =
                new Regex("(?<LinkTypeReferenceName>.*)-(?<Direction>.*)", RegexOptions.Singleline | RegexOptions.Compiled);

        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTeamProjectCollection> _tfs;

        private IFieldDefinitionCollection _fieldDefinitions;

        private IWorkItemLinkTypeCollection _linkTypes;

        private IProjectCollection _projects;

        internal WorkItemStore(Func<IInternalTeamProjectCollection> tpcFactory, Func<WorkItemStore, IQueryFactory> queryFactory)
            : this(tpcFactory, () => tpcFactory()?.GetClient<WorkItemTrackingHttpClient>(), queryFactory)
        {
        }

        internal WorkItemStore(
            Func<IInternalTeamProjectCollection> tpcFactory,
            Func<WorkItemTrackingHttpClient> wisFactory,
            Func<WorkItemStore, IQueryFactory> queryFactory)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));
            _tfs = new Lazy<IInternalTeamProjectCollection>(tpcFactory);
            NativeWorkItemStore = new Lazy<WorkItemTrackingHttpClient>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory(this));
            Configuration = new WorkItemStoreConfiguration();
        }

        public VssCredentials AuthorizedCredentials => TeamProjectCollection.AuthorizedCredentials;

        public ITeamFoundationIdentity AuthorizedIdentity => TeamProjectCollection.AuthorizedIdentity;

        /// <inheritdoc />
        Qwiq.WorkItemStoreConfiguration IWorkItemStore.Configuration => Configuration;

        public WorkItemStoreConfiguration Configuration { get; }

        public IFieldDefinitionCollection FieldDefinitions => _fieldDefinitions
                                                              ?? (_fieldDefinitions = new FieldDefinitionCollection(this));

        public IProjectCollection Projects => _projects ?? (_projects = ProjectCollectionFactory());

        public IRegisteredLinkTypeCollection RegisteredLinkTypes { get; }

        public ITeamProjectCollection TeamProjectCollection => _tfs.Value;

        public TimeZone TimeZone => TeamProjectCollection?.TimeZone ?? TimeZone.CurrentTimeZone;

        public IWorkItemLinkTypeCollection WorkItemLinkTypes => _linkTypes ?? (_linkTypes = WorkItemLinkTypeCollectionFactory());

        internal Lazy<WorkItemTrackingHttpClient> NativeWorkItemStore { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IWorkItemCollection Query(string wiql, bool dayPrecision = false)
        {
            // REVIEW: SOAP client catches a ValidationException here
            var query = _queryFactory.Value.Create(wiql, dayPrecision);
            return query.RunQuery();
        }

        public IWorkItemCollection Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            // Same behavior as SOAP version
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var ids2 = (int[])ids.ToArray().Clone();
            if (!ids2.Any()) return Enumerable.Empty<IWorkItem>().ToWorkItemCollection();

            var query = _queryFactory.Value.Create(ids2, asOf);
            return query.RunQuery();
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            return Query(new[] { id }, asOf).SingleOrDefault();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            // REVIEW: SOAP client catches a ValidationException here
            var query = _queryFactory.Value.Create(wiql, dayPrecision);
            return query.RunLinkQuery();
        }

        private static WorkItemLinkTypeCollection GetLinks(WorkItemTrackingHttpClient workItemStore)
        {
            var types = workItemStore.GetRelationTypesAsync().GetAwaiter().GetResult();
            var d = new Dictionary<string, IList<WorkItemRelationType>>(StringComparer.OrdinalIgnoreCase);
            var d2 = new Dictionary<string, WorkItemLinkType>(StringComparer.OrdinalIgnoreCase);

            foreach (var type in types.Where(p => (string)p.Attributes["usage"] == "workItemLink"))
            {
                var m = ImmutableLinkTypeNameRegex.Match(type.ReferenceName);
                var linkRef = m.Groups["LinkTypeReferenceName"].Value;

                if (string.IsNullOrWhiteSpace(linkRef)) linkRef = type.ReferenceName;

                if (!d.ContainsKey(linkRef)) d[linkRef] = new List<WorkItemRelationType>();
                d[linkRef].Add(type);

                if (!d2.ContainsKey(linkRef)) d2[linkRef] = new WorkItemLinkType(linkRef);
            }

            foreach (var kvp in d2)
            {
                var type = kvp.Value;
                var ends = d[kvp.Key];

                type.IsDirectional = ends.All(p => (bool)p.Attributes["directional"]);
                type.IsActive = ends.All(p => (bool)p.Attributes["enabled"]);

                var forwardEnd = ends.Count == 1 && !type.IsDirectional
                                     ? ends[0]
                                     : ends.SingleOrDefault(p => p.ReferenceName.EndsWith("Forward"));

                if (!forwardEnd.ReferenceName.EndsWith("Forward")) forwardEnd.ReferenceName += "-Forward";

                type.SetForwardEnd(new WorkItemLinkTypeEnd(forwardEnd) { IsForwardLink = true, LinkType = type });
                type.SetReverseEnd(
                                   type.IsDirectional
                                       ? new WorkItemLinkTypeEnd(
                                                                 ends.SingleOrDefault(p => p.ReferenceName.EndsWith("Reverse")))
                                       {
                                           LinkType
                                                         = type
                                       }
                                       : type.ForwardEnd);

                // The REST API does not return the ID of the link type. For well-known system links, we can populate the ID value
                if (CoreLinkTypeReferenceNames.All.Contains(type.ReferenceName, StringComparer.OrdinalIgnoreCase))
                {
                    int forwardId = 0,
                        reverseId = 0;

                    if (CoreLinkTypeReferenceNames.Hierarchy.Equals(type.ReferenceName, StringComparison.OrdinalIgnoreCase))
                    {
                        // The forward should be Child, but the ID used in CoreLinkTypes is -2, should be 2
                        forwardId = CoreLinkTypes.Child;
                        reverseId = CoreLinkTypes.Parent;
                    }
                    else if (CoreLinkTypeReferenceNames.Related.Equals(type.ReferenceName, StringComparison.OrdinalIgnoreCase))
                    {
                        forwardId = reverseId = CoreLinkTypes.Related;
                    }
                    else if (CoreLinkTypeReferenceNames.Dependency.Equals(type.ReferenceName, StringComparison.OrdinalIgnoreCase))
                    {
                        forwardId = CoreLinkTypes.Successor;
                        reverseId = CoreLinkTypes.Predecessor;
                    }

                    ((WorkItemLinkTypeEnd)type.ForwardEnd).Id = -forwardId;
                    ((WorkItemLinkTypeEnd)type.ReverseEnd).Id = -reverseId;
                }
            }

            return new WorkItemLinkTypeCollection(d2.Values.OfType<IWorkItemLinkType>().ToList());
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (NativeWorkItemStore != null && NativeWorkItemStore.IsValueCreated)
                {
                    NativeWorkItemStore?.Value?.Dispose();
                    NativeWorkItemStore = null;
                }
            }
        }

        private IProjectCollection ProjectCollectionFactory()
        {
            const string continuationHeader = "x-ms-continuationtoken";

            // See https://www.visualstudio.com/en-us/docs/integrate/api/tfs/projects
            const ProjectState defaultStateFilter = ProjectState.WellFormed;
            const int defaultNumberOfTeamProjects = 100;

            // Use the page size configured by the client if it is higher than the default
            // Otherwise, with a default of 50, we would need to make multiple trips to load all the data
            var pageSize = Math.Max(defaultNumberOfTeamProjects, Configuration.PageSize);

            using (var projectHttpClient = _tfs.Value.GetClient<ProjectHttpClient>())
            {
                var projects = new List<TeamProjectReference>(pageSize);

                var projectReferences = projectHttpClient.GetProjects(defaultStateFilter, pageSize).GetAwaiter().GetResult();
                projects.AddRange(projectReferences);
                while (projectHttpClient.LastResponseContext.Headers.Contains(continuationHeader))
                {
                    projectReferences = projectHttpClient.GetProjects(defaultStateFilter, pageSize, projects.Count).GetAwaiter().GetResult();
                    projects.AddRange(projectReferences);
                }

                var projects2 = new List<IProject>(projects.Count + 1);

                for (var i = 0; i < projects.Count; i++)
                {
                    var project = projects[i];
                    var p = new Project(project, this);
                    projects2.Add(p);
                }

                return new ProjectCollection(projects2);
            }
        }

        private WorkItemLinkTypeCollection WorkItemLinkTypeCollectionFactory()
        {
            return GetLinks(NativeWorkItemStore.Value);
        }
    }
}