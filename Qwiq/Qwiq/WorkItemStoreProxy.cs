using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.IE.Qwiq
{
    /// <summary>
    /// Wrapper around the TFS WorkItemStore. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly Tfs.Client.TfsTeamProjectCollection _tfs;
        private readonly Tfs.WorkItemTracking.Client.WorkItemStore _workItemStore;

        // To do: stub out the following
        public ITfsTeamProjectCollection TeamProjectCollection
        {
            get { return new TfsTeamProjectCollectionProxy(_workItemStore.TeamProjectCollection); }
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

        internal WorkItemStoreProxy(Uri endpoint, Tfs.Client.TfsClientCredentials credentials)
        {
            _tfs = new Tfs.Client.TfsTeamProjectCollection(endpoint, credentials);
            _workItemStore = _tfs.GetService<Tfs.WorkItemTracking.Client.WorkItemStore>();
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql)
        {
            var query = new Tfs.WorkItemTracking.Client.Query(_workItemStore, wiql);
            return query.RunLinkQuery().Select(item => new WorkItemLinkInfoProxy(item));
        }

        public IEnumerable<IWorkItem> Query(string wiql)
        {
            return
                _workItemStore.Query(wiql)
                    .Cast<Tfs.WorkItemTracking.Client.WorkItem>()
                    .Select(item => new WorkItemProxy(item));
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids)
        {
            const string wiql = "SELECT * FROM WorkItems WHERE [System.ID] IN ({0})";
            var query = string.Format(CultureInfo.InvariantCulture, wiql, string.Join(",", ids));

            return Query(query);
        }

        public IWorkItem Query(int id)
        {
            return Query(new[] { id }).SingleOrDefault();
        }

        public IWorkItem Create(string type, string projectName)
        {
            var wits = _workItemStore.Projects[projectName].WorkItemTypes;
            return new WorkItemProxy(new Tfs.WorkItemTracking.Client.WorkItem(wits[type]));
        }
    }
}
