using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class WorkItemStore : IWorkItemStore
    {
        private static readonly Regex ImmutableLinkTypeNameRegex = new Regex(
            "(?<LinkTypeReferenceName>.*)-(?<Direction>.*)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private IWorkItemLinkTypeCollection _linkTypes;

        private IProjectCollection _projects;

        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTeamProjectCollection> _tfs;

        private IFieldDefinitionCollection _fieldDefinitions;

        internal WorkItemStore(
            Func<IInternalTeamProjectCollection> tpcFactory,
            Func<WorkItemStore, IQueryFactory> queryFactory,
            int pageSize = Rest.Query.MaximumBatchSize)
            : this(tpcFactory, () => tpcFactory()?.GetClient<WorkItemTrackingHttpClient>(), queryFactory, pageSize)
        {
        }

        internal WorkItemStore(
            Func<IInternalTeamProjectCollection> tpcFactory,
            Func<WorkItemTrackingHttpClient> wisFactory,
            Func<WorkItemStore, IQueryFactory> queryFactory,
            int pageSize = Rest.Query.MaximumBatchSize)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));
            _tfs = new Lazy<IInternalTeamProjectCollection>(tpcFactory);
            NativeWorkItemStore = new Lazy<WorkItemTrackingHttpClient>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory(this));

            // Boundary check the batch size

            if (pageSize < Rest.Query.MinimumBatchSize || pageSize > Rest.Query.MaximumBatchSize) throw new PageSizeRangeException();


            PageSize = pageSize;





        }

        private WorkItemLinkTypeCollection WorkItemLinkTypeCollectionFactory()
        {
            return GetLinks(NativeWorkItemStore.Value);
        }

        private IProjectCollection ProjectCollectionFactory()
        {
            using (var projectHttpClient = _tfs.Value.GetClient<ProjectHttpClient>())
            {
                var projects = (List<TeamProjectReference>)projectHttpClient.GetProjects(ProjectState.All).GetAwaiter().GetResult();
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

        public int PageSize { get; }

        internal Lazy<WorkItemTrackingHttpClient> NativeWorkItemStore { get; }

        public VssCredentials AuthorizedCredentials => TeamProjectCollection.AuthorizedCredentials;

        public IFieldDefinitionCollection FieldDefinitions => _fieldDefinitions ?? (_fieldDefinitions = new FieldDefinitionCollection(this));

        public IProjectCollection Projects => _projects ?? (_projects = ProjectCollectionFactory());

        public ITeamProjectCollection TeamProjectCollection => _tfs.Value;

        public ITeamFoundationIdentity AuthorizedIdentity => TeamProjectCollection.AuthorizedIdentity;

        public TimeZone TimeZone => TeamProjectCollection?.TimeZone ?? TimeZone.CurrentTimeZone;

        public IWorkItemLinkTypeCollection WorkItemLinkTypes => _linkTypes ?? (_linkTypes = WorkItemLinkTypeCollectionFactory());

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

        public IRegisteredLinkTypeCollection RegisteredLinkTypes { get; }

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
                              ends.SingleOrDefault(p => p.ReferenceName.EndsWith("Reverse"))) { LinkType = type }
                        : type.ForwardEnd);

                // The REST API does not return the ID of the link type. For well-known system links, we can populate the ID value
                if (CoreLinkTypeReferenceNames.All.Contains(type.ReferenceName, StringComparer.OrdinalIgnoreCase))
                {
                    int forwardId = 0, reverseId = 0;

                    if (CoreLinkTypeReferenceNames.Hierarchy.Equals(
                        type.ReferenceName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        // The forward should be Child, but the ID used in CoreLinkTypes is -2, should be 2
                        forwardId = CoreLinkTypes.Child;
                        reverseId = CoreLinkTypes.Parent;
                    }
                    else if (CoreLinkTypeReferenceNames.Related.Equals(
                        type.ReferenceName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        forwardId = reverseId = CoreLinkTypes.Related;
                    }
                    else if (CoreLinkTypeReferenceNames.Dependency.Equals(
                        type.ReferenceName,
                        StringComparison.OrdinalIgnoreCase))
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
            if (disposing) if (NativeWorkItemStore.IsValueCreated) NativeWorkItemStore.Value?.Dispose();
        }
    }
}