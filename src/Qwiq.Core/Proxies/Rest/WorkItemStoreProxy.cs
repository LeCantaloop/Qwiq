using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Rest;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly int _batchSize;

        private readonly Regex _immutableLinkTypeNameRegex = new Regex(
            "(?<LinkTypeReferenceName>.*)-(?<Direction>.*)",
            RegexOptions.Singleline | RegexOptions.Compiled);

        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTfsTeamProjectCollection> _tfs;

        private readonly Lazy<IEnumerable<IWorkItemLinkType>> _linkTypes;

        internal WorkItemStoreProxy(
            TfsTeamProjectCollection tfsNative,
            Func<WorkItemTrackingHttpClient, IQueryFactory> queryFactory,
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
            Func<WorkItemTrackingHttpClient, IQueryFactory> queryFactory,
            int batchSize = QueryProxy.MaximumBatchSize)
            : this(tpcFactory, () => tpcFactory()?.GetClient<WorkItemTrackingHttpClient>(), queryFactory, batchSize)
        {
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<WorkItemTrackingHttpClient> wisFactory,
            Func<WorkItemTrackingHttpClient, IQueryFactory> queryFactory,
            int batchSize = QueryProxy.MaximumBatchSize)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));
            _tfs = new Lazy<IInternalTfsTeamProjectCollection>(tpcFactory);
            var workItemStore = new Lazy<WorkItemTrackingHttpClient>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory.Invoke(workItemStore?.Value));

            // Boundary check the batch size

            if (batchSize < QueryProxy.MinimumBatchSize) throw new PageSizeRangeException();
            if (batchSize > QueryProxy.MaximumBatchSize) throw new PageSizeRangeException();

            _batchSize = batchSize;

            IEnumerable<IWorkItemLinkType> ValueFactory()
            {
                var types = workItemStore.Value.GetRelationTypesAsync().GetAwaiter().GetResult();
                var d = new Dictionary<string, IList<WorkItemRelationType>>(StringComparer.OrdinalIgnoreCase);
                var d2 = new Dictionary<string, WorkItemLinkTypeProxy>(StringComparer.OrdinalIgnoreCase);

                foreach (var type in types.Where(p => (string)p.Attributes["usage"] == "workItemLink"))
                {
                    var m = _immutableLinkTypeNameRegex.Match(type.ReferenceName);
                    var linkRef = m.Groups["LinkTypeReferenceName"].Value;

                    if (string.IsNullOrWhiteSpace(linkRef))
                    {
                        linkRef = type.ReferenceName;
                    }

                    if (!d.ContainsKey(linkRef)) d[linkRef] = new List<WorkItemRelationType>();
                    d[linkRef].Add(type);

                    if (!d2.ContainsKey(linkRef)) d2[linkRef] = new WorkItemLinkTypeProxy(linkRef);
                }

                foreach (var kvp in d2)
                {
                    var type = kvp.Value;
                    var ends = d[kvp.Key];

                    var isDirectional = ends.All(p => (bool)p.Attributes["directional"]);

                    type.IsActive = ends.All(p => (bool)p.Attributes["enabled"]);

                    var forwardEnd = ends.Count == 1 && !isDirectional
                                         ? ends[0]
                                         : ends.SingleOrDefault(p => p.ReferenceName.EndsWith("Forward"));

                    if (!forwardEnd.ReferenceName.EndsWith("Forward"))
                    {
                        forwardEnd.ReferenceName += "-Forward";
                    }

                    type.ForwardEnd = new WorkItemLinkTypeEndProxy(forwardEnd) { IsForwardLink = true, LinkType = type };
                    if (isDirectional)
                    {
                        type.ReverseEnd = new WorkItemLinkTypeEndProxy(ends.SingleOrDefault(p => p.ReferenceName.EndsWith("Reverse"))) { LinkType = type };
                        ((WorkItemLinkTypeEndProxy)type.ReverseEnd).OppositeEnd = type.ForwardEnd;
                        ((WorkItemLinkTypeEndProxy)type.ForwardEnd).OppositeEnd = type.ReverseEnd;
                    }
                    else
                    {
                        ((WorkItemLinkTypeEndProxy)type.ForwardEnd).OppositeEnd = type.ForwardEnd;
                        type.ReverseEnd = type.ForwardEnd;
                    }

                    yield return type;
                }
            }

            _linkTypes = new Lazy<IEnumerable<IWorkItemLinkType>>(ValueFactory);
        }

        public TfsCredentials AuthorizedCredentials => TeamProjectCollection.AuthorizedCredentials;

        public IFieldDefinitionCollection FieldDefinitions => throw new NotImplementedException();

        public IEnumerable<IProject> Projects => throw new NotImplementedException();

        public ITfsTeamProjectCollection TeamProjectCollection => _tfs.Value;

        public TimeZone TimeZone => TeamProjectCollection?.TimeZone ?? TimeZone.CurrentTimeZone;

        // REVIEW: SOAP WorkItemStore gets the identity from its cache based on UserSid
        public string UserDisplayName => throw new NotImplementedException();

        // REVIEW: SOAP WorkItemStore gets the identity from its cache based on UserSid
        public string UserIdentityName => throw new NotImplementedException();

        public string UserSid => TeamProjectCollection.AuthorizedIdentity.Descriptor.Identifier;

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes => _linkTypes.Value;

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

        private void Dispose(bool disposing)
        {
            if (disposing) if (_tfs.IsValueCreated) _tfs.Value?.Dispose();
        }
    }
}