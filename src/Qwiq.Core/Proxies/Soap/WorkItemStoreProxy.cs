using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.Client;


using TfsWorkItem = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    /// <summary>
    /// Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly Lazy<IQueryFactory> _queryFactory;

        private readonly Lazy<IInternalTfsTeamProjectCollection> _tfs;

        private readonly Lazy<TfsWorkItem.WorkItemStore> _workItemStore;

        internal WorkItemStoreProxy(
            TfsTeamProjectCollection tfsNative,
            Func<TfsWorkItem.WorkItemStore, IQueryFactory> queryFactory)
            : this(
                () =>
                    ExceptionHandlingDynamicProxyFactory.Create<IInternalTfsTeamProjectCollection>(
                        new TfsTeamProjectCollectionProxy(tfsNative)),
                queryFactory)
        {
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<TfsWorkItem.WorkItemStore> wisFactory,
            Func<TfsWorkItem.WorkItemStore, IQueryFactory> queryFactory)
        {
            if (tpcFactory == null) throw new ArgumentNullException(nameof(tpcFactory));
            if (wisFactory == null) throw new ArgumentNullException(nameof(wisFactory));
            if (queryFactory == null) throw new ArgumentNullException(nameof(queryFactory));
            _tfs = new Lazy<IInternalTfsTeamProjectCollection>(tpcFactory);
            _workItemStore = new Lazy<TfsWorkItem.WorkItemStore>(wisFactory);
            _queryFactory = new Lazy<IQueryFactory>(() => queryFactory.Invoke(_workItemStore?.Value));
        }

        internal WorkItemStoreProxy(
            Func<IInternalTfsTeamProjectCollection> tpcFactory,
            Func<TfsWorkItem.WorkItemStore, IQueryFactory> queryFactory)
            : this(tpcFactory, () => tpcFactory()?.GetService<TfsWorkItem.WorkItemStore>(), queryFactory)
        {
        }

        public TfsCredentials AuthorizedCredentials => _tfs.Value.AuthorizedCredentials;

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tfs.IsValueCreated) _tfs.Value?.Dispose();
            }
        }

        #endregion IDisposable

        public IEnumerable<IProject> Projects
        {
            get
            {
                return
                    _workItemStore.Value.Projects.Cast<TfsWorkItem.Project>()
                                  .Select(
                                      item =>
                                          ExceptionHandlingDynamicProxyFactory.Create<IProject>(new ProjectProxy(item)));
            }
        }

        public ITfsTeamProjectCollection TeamProjectCollection => _tfs.Value;

        public TimeZone TimeZone => _workItemStore.Value.TimeZone;

        public string UserDisplayName => _workItemStore.Value.UserDisplayName;

        public string UserIdentityName => _workItemStore.Value.UserIdentityName;

        public string UserSid => _workItemStore.Value.UserSid;

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes
        {
            get
            {
                return
                    _workItemStore.Value.WorkItemLinkTypes.Select(
                        item =>
                            ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkType>(
                                new WorkItemLinkTypeProxy(item)));
            }
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            try
            {
                var query = _queryFactory.Value.Create(wiql, dayPrecision);
                return query.RunQuery();
            }
            catch (TfsWorkItem.ValidationException ex)
            {
                throw new ValidationException(ex);
            }
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (!ids.Any())
            {
                return Enumerable.Empty<IWorkItem>();
            }

            var wiql = "SELECT * FROM WorkItems WHERE [System.Id] IN ({0})";
            if (asOf.HasValue)
            {
                wiql += " ASOF '" + asOf.Value.ToString("u") + "'";
            }

            var query = string.Format(CultureInfo.InvariantCulture, wiql, string.Join(", ", ids));

            return Query(query);
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            return Query(new[] { id }, asOf).SingleOrDefault();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            try
            {
                var query = _queryFactory.Value.Create(wiql, dayPrecision);
                return query.RunLinkQuery();
            }
            catch (TfsWorkItem.ValidationException ex)
            {
                throw new ValidationException(ex);
            }
        }
    }
}