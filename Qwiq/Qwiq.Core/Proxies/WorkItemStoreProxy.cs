using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Qwiq.Exceptions;
using TfsWorkItem = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    /// <summary>
    /// Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly IQueryFactory _queryFactory;
        private readonly IInternalTfsTeamProjectCollection _tfs;
        private readonly TfsWorkItem.WorkItemStore _workItemStore;

        internal WorkItemStoreProxy(IInternalTfsTeamProjectCollection tfs, TfsWorkItem.WorkItemStore workItemStore, IQueryFactory queryFactory)
        {
            _tfs = tfs;
            _workItemStore = workItemStore;
            _queryFactory = queryFactory;

        }

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
                _tfs.Dispose();
            }
        }
        #endregion

        public ITfsTeamProjectCollection TeamProjectCollection
        {
            get { return _tfs; }
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            try
            {
                var query = _queryFactory.Create(wiql, dayPrecision);
                return query.RunLinkQuery();
            }
            catch (TfsWorkItem.ValidationException ex)
            {
                throw new ValidationException(ex);
            }
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            try
            {
                var query = _queryFactory.Create(wiql, dayPrecision);
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

        public IEnumerable<IProject> Projects
        {
            get
            {
                return
                    _workItemStore.Projects.Cast<TfsWorkItem.Project>()
                        .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IProject>(new ProjectProxy(item)));
            }
        }

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes
        {
            get
            {
                return
                    _workItemStore.WorkItemLinkTypes
                        .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkType>(new WorkItemLinkTypeProxy(item)));
            }
        }

        public TimeZone TimeZone => _workItemStore.TimeZone;
    }


}

