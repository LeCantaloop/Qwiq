using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.IE.Qwiq.Credentials;
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

        public WorkItemStoreProxy(Uri endpoint, TfsCredentials credentials)
        {
            _tfs = new Tfs.Client.TfsTeamProjectCollection(endpoint, credentials.Credentials);
            _workItemStore = _tfs.GetService<Tfs.WorkItemTracking.Client.WorkItemStore>();
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
            get { return new TfsTeamProjectCollectionProxy(_workItemStore.TeamProjectCollection); }
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql)
        {
            var query = new Tfs.WorkItemTracking.Client.Query(_workItemStore, wiql);
            return query.RunLinkQuery().Select(item => new WorkItemLinkInfoProxy(item));
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, IDictionary context, bool dayPrecision)
        {
            var query = new Tfs.WorkItemTracking.Client.Query(_workItemStore, wiql, context, dayPrecision);
            return query.RunLinkQuery().Select(item => new WorkItemLinkInfoProxy(item));
        }

        public IEnumerable<IWorkItem> Query(string wiql, IDictionary context, bool dayPrecision)
        {
            var query = new Tfs.WorkItemTracking.Client.Query(_workItemStore, wiql, context, dayPrecision);
            return query.RunQuery().Cast<Tfs.WorkItemTracking.Client.WorkItem>().Select(item => new WorkItemProxy(item));
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

        public IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem)
        {
            var concreteLinkTypeEnd = _workItemStore.WorkItemLinkTypes.LinkTypeEnds[linkTypeEnd.ImmutableName];
            var link = new Tfs.WorkItemTracking.Client.RelatedLink(concreteLinkTypeEnd, relatedWorkItem.Id);

            return new RelatedLinkProxy(link);
        }
    }
}
