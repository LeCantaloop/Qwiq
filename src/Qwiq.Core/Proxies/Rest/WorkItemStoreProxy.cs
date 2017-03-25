using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private static readonly Regex ImmutableLinkTypeNameRegex = new Regex(
            "(?<LinkTypeReferenceName>.*)-(?<Direction>.*)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public int BatchSize { get; }

        private readonly Lazy<WorkItemLinkTypeCollection> _linkTypes;

        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTfsTeamProjectCollection> _tfs;

        internal Lazy<WorkItemTrackingHttpClient> NativeWorkItemStore { get; }

        private readonly Lazy<IEnumerable<IProject>> _projects;

        internal WorkItemStoreProxy(
            TfsTeamProjectCollection tfsNative,
            Func<WorkItemStoreProxy, IQueryFactory> queryFactory,
            int batchSize = QueryProxy.MaximumBatchSize)
            : this(
                () => ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                    new TfsTeamProjectCollectionProxy(tfsNative)),
                queryFactory,
                batchSize)
        {
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<WorkItemStoreProxy, IQueryFactory> queryFactory,
            int batchSize = QueryProxy.MaximumBatchSize)
            : this(tpcFactory, () => tpcFactory()?.GetClient<WorkItemTrackingHttpClient>(), queryFactory, batchSize)
        {
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<WorkItemTrackingHttpClient> wisFactory,
            Func<WorkItemStoreProxy, IQueryFactory> queryFactory,
            int batchSize = QueryProxy.MaximumBatchSize)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));
            _tfs = new Lazy<IInternalTfsTeamProjectCollection>(tpcFactory);
            NativeWorkItemStore = new Lazy<WorkItemTrackingHttpClient>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory(this));

            // Boundary check the batch size

            if (batchSize < QueryProxy.MinimumBatchSize) throw new PageSizeRangeException();
            if (batchSize > QueryProxy.MaximumBatchSize) throw new PageSizeRangeException();

            BatchSize = batchSize;

            WorkItemLinkTypeCollection ValueFactory()
            {
                return GetLinks(NativeWorkItemStore.Value);
            }

            _linkTypes = new Lazy<WorkItemLinkTypeCollection>(ValueFactory);
            _projects = new Lazy<IEnumerable<IProject>>(
                () =>
                    {
                        using (var projectHttpClient = _tfs.Value.GetClient<ProjectHttpClient>())
                        {
                            var projects = projectHttpClient.GetProjects(ProjectState.WellFormed).GetAwaiter().GetResult();
                            return projects.Select(project => new ProjectProxy(project, this))
                                           .Cast<IProject>()
                                           .ToList();
                        }
                    });
        }

        public TfsCredentials AuthorizedCredentials => TeamProjectCollection.AuthorizedCredentials;

        public IFieldDefinitionCollection FieldDefinitions => throw new NotImplementedException();

        public IEnumerable<IProject> Projects => _projects.Value;

        public ITfsTeamProjectCollection TeamProjectCollection => _tfs.Value;

        public TimeZone TimeZone => TeamProjectCollection?.TimeZone ?? TimeZone.CurrentTimeZone;

        // REVIEW: SOAP WorkItemStore gets the identity from its cache based on UserSid
        public string UserDisplayName => throw new NotImplementedException();

        // REVIEW: SOAP WorkItemStore gets the identity from its cache based on UserSid
        public string UserIdentityName => throw new NotImplementedException();

        public string UserSid => TeamProjectCollection.AuthorizedIdentity.Descriptor.Identifier;

        public WorkItemLinkTypeCollection WorkItemLinkTypes => _linkTypes.Value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            if (dayPrecision) throw new NotSupportedException();

            // REVIEW: SOAP client catches a ValidationException here
            var query = _queryFactory.Value.Create(wiql, dayPrecision);
            return query.RunQuery();
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            var ids2 = (int[])ids.ToArray().Clone();

            // The WIQL's WHERE and ORDER BY clauses are not used to filter (as we have specified IDs).
            // It is used for ASOF
            var wiql = "SELECT * FROM WorkItems";
            if (asOf.HasValue)
            {
                // If specified DateTime is not UTC convert it to local time based on TFS client TimeZone
                if (asOf.Value.Kind != DateTimeKind.Utc)
                    asOf = DateTime.SpecifyKind(asOf.Value - TimeZone.GetUtcOffset(asOf.Value), DateTimeKind.Utc);
                wiql += $" ASOF \'{asOf.Value:u}\'";
            }

            var query = _queryFactory.Value.Create(ids2, wiql);
            return query.RunQuery();
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            return Query(new[] { id }, asOf).SingleOrDefault();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            if (dayPrecision) throw new NotSupportedException();

            // REVIEW: SOAP client catches a ValidationException here
            var query = _queryFactory.Value.Create(wiql, dayPrecision);
            return query.RunLinkQuery();
        }

        internal static WorkItemLinkTypeCollection GetLinks(WorkItemTrackingHttpClient workItemStore)
        {
            var types = workItemStore.GetRelationTypesAsync().GetAwaiter().GetResult();
            var d = new Dictionary<string, IList<WorkItemRelationType>>(StringComparer.OrdinalIgnoreCase);
            var d2 = new Dictionary<string, WorkItemLinkTypeProxy>(StringComparer.OrdinalIgnoreCase);

            foreach (var type in types.Where(p => (string)p.Attributes["usage"] == "workItemLink"))
            {
                var m = ImmutableLinkTypeNameRegex.Match(type.ReferenceName);
                var linkRef = m.Groups["LinkTypeReferenceName"].Value;

                if (string.IsNullOrWhiteSpace(linkRef)) linkRef = type.ReferenceName;

                if (!d.ContainsKey(linkRef)) d[linkRef] = new List<WorkItemRelationType>();
                d[linkRef].Add(type);

                if (!d2.ContainsKey(linkRef)) d2[linkRef] = new WorkItemLinkTypeProxy(linkRef);
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

                type.ForwardEnd = new WorkItemLinkTypeEndProxy(forwardEnd) { IsForwardLink = true, LinkType = type };
                type.ReverseEnd = type.IsDirectional
                                      ? new WorkItemLinkTypeEndProxy(
                                            ends.SingleOrDefault(
                                                p => p.ReferenceName.EndsWith("Reverse")))
                                      { LinkType = type }
                                      : type.ForwardEnd;

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

                    ((WorkItemLinkTypeEndProxy)type.ForwardEnd).Id = -forwardId;
                    ((WorkItemLinkTypeEndProxy)type.ReverseEnd).Id = -reverseId;
                }
            }

            return new WorkItemLinkTypeCollection(d2.Values);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tfs.IsValueCreated) _tfs.Value?.Dispose();
                if (NativeWorkItemStore.IsValueCreated) NativeWorkItemStore.Value?.Dispose();
            }
        }
    }
}