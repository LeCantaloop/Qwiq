using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Rest;
using Microsoft.Qwiq.Rest;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly int _batchSize;

        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTfsTeamProjectCollection> _tfs;

        private readonly Lazy<WorkItemTrackingHttpClient> _workItemStore;

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
            _workItemStore = new Lazy<WorkItemTrackingHttpClient>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory.Invoke(_workItemStore?.Value));


            // Boundary check the batch size

            if (batchSize < QueryProxy.MinimumBatchSize) throw new PageSizeRangeException();
            if (batchSize > QueryProxy.MaximumBatchSize) throw new PageSizeRangeException();

            _batchSize = batchSize;
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

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes => throw new NotImplementedException();

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
                {
                    asOf = DateTime.SpecifyKind(asOf.Value - TimeZone.GetUtcOffset(asOf.Value), DateTimeKind.Utc);
                }
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