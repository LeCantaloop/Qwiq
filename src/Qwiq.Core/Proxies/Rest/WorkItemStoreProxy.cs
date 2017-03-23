using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTfsTeamProjectCollection> _tfs;

        private readonly Lazy<WorkItemTrackingHttpClient> _workItemStore;

        internal WorkItemStoreProxy(
            TfsTeamProjectCollection tfsNative,
            Func<WorkItemTrackingHttpClient, IQueryFactory> queryFactory)
            : this(
                () => ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                    new TfsTeamProjectCollectionProxy(tfsNative)),
                queryFactory)
        {
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<WorkItemTrackingHttpClient, IQueryFactory> queryFactory)
            : this(tpcFactory, () => tpcFactory()?.GetClient<WorkItemTrackingHttpClient>(), queryFactory)
        {
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<WorkItemTrackingHttpClient> wisFactory,
            Func<WorkItemTrackingHttpClient, IQueryFactory> queryFactory)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));
            _tfs = new Lazy<IInternalTfsTeamProjectCollection>(tpcFactory);
            _workItemStore = new Lazy<WorkItemTrackingHttpClient>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory.Invoke(_workItemStore?.Value));
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
            if (!ids.Any()) return Enumerable.Empty<IWorkItem>();

            // REVIEW: This implementation is the same as SOAP but requires two trips to the server
            // First trip to execute the WIQL and return the IDs,
            // the second trip to load items by ID

            var wiql = "SELECT * FROM WorkItems WHERE [System.Id] IN ({0})";
            if (asOf.HasValue) wiql += " ASOF '" + asOf.Value.ToString("u") + "'";

            var query = string.Format(CultureInfo.InvariantCulture, wiql, string.Join(", ", ids));

            return Query(query);
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