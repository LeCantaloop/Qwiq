using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.IE.Qwiq.Credentials;
using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.IE.Qwiq.Proxies
{
    /// <summary>
    /// Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly IQueryFactory _queryFactory;
        private readonly Tfs.Client.TfsTeamProjectCollection _tfs;
        private readonly Tfs.WorkItemTracking.Client.WorkItemStore _workItemStore;

        internal WorkItemStoreProxy(Tfs.Client.TfsTeamProjectCollection tfs, Tfs.WorkItemTracking.Client.WorkItemStore workItemStore, IQueryFactory queryFactory)
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
            get { return new TfsTeamProjectCollectionProxy(_tfs); }
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            var query = _queryFactory.Create(wiql, dayPrecision);
            return query.RunLinkQuery();
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            var query = _queryFactory.Create(wiql, dayPrecision);
            return query.RunQuery();
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
                    _workItemStore.Projects.Cast<Tfs.WorkItemTracking.Client.Project>()
                        .Select(item => new ProjectProxy(item));
            }
        }

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes
        {
            get
            {
                return
                    _workItemStore.WorkItemLinkTypes
                        .Select(item => new WorkItemLinkTypeProxy(item));
            }
        }
    }

    
}
