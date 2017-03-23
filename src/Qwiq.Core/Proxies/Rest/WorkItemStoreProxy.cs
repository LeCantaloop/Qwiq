using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Credentials;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemStoreProxy : IWorkItemStore
    {
        private readonly WorkItemTrackingHttpClient _workItemStore;

        private readonly int _batchSize;

        private IInternalTfsTeamProjectCollection _teamProjectCollection;

        internal WorkItemStoreProxy(
            IInternalTfsTeamProjectCollection teamProjectCollection,
            WorkItemTrackingHttpClient workItemStore,
            int batchSize = 100)
        {
            if (teamProjectCollection == null) throw new ArgumentNullException(nameof(teamProjectCollection));
            if (workItemStore == null) throw new ArgumentNullException(nameof(workItemStore));

            _teamProjectCollection = teamProjectCollection;
            _workItemStore = workItemStore;

            // Boundary check the batch size
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, "Batch size must be greater than 0.");
            if (batchSize > 200) throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, "Batch size must be less than 200.");
            _batchSize = batchSize;
        }

        public TfsCredentials AuthorizedCredentials => throw new NotImplementedException();

        public IEnumerable<IProject> Projects => throw new NotImplementedException();

        public ITfsTeamProjectCollection TeamProjectCollection => _teamProjectCollection;

        public TimeZone TimeZone => throw new NotImplementedException();

        public string UserDisplayName => throw new NotImplementedException();

        public string UserIdentityName => throw new NotImplementedException();

        public string UserSid => throw new NotImplementedException();

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes => throw new NotImplementedException();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            if (dayPrecision) throw new NotSupportedException();

            var p = Parser.ParseSyntax(wiql);
            var w = new Wiql { Query = p.ToString() };

            var fields = new List<string>();
            for (var i = 0; i < p.Fields.Count; i++)
            {
                var field = p.Fields[i];
                fields.Add(field.Value);
            }

            var result = _workItemStore.QueryByWiqlAsync(w).GetAwaiter().GetResult();
            if (result.WorkItems.Any())
            {
                var skip = 0;
                IEnumerable<WorkItemReference> workItemRefs;
                do
                {
                    workItemRefs = result.WorkItems.Skip(skip).Take(_batchSize).ToList();
                    if (workItemRefs.Any())
                    {
                        // TODO: Support AsOf
                        var workItems = _workItemStore
                            .GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id), fields, null, null)
                            .GetAwaiter()
                            .GetResult();
                        foreach (var workItem in workItems) yield return new WorkItemProxy(workItem);
                    }
                    skip += _batchSize;
                }
                while (workItemRefs.Count() == _batchSize);
            }
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (asOf.HasValue) throw new NotSupportedException();

            if (!ids.Any()) yield return null;

            var wis = _workItemStore.GetWorkItemsAsync(ids, null, asOf, WorkItemExpand.Fields).GetAwaiter().GetResult();
            foreach (var workItem in wis)
                // write work item to console
                yield return new WorkItemProxy(workItem);
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            if (asOf.HasValue) throw new NotSupportedException();

            var wi = _workItemStore.GetWorkItemAsync(id, null, asOf, WorkItemExpand.Fields).GetAwaiter().GetResult();
            return new WorkItemProxy(wi);
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            if (dayPrecision) throw new NotSupportedException();

            var w = new Wiql { Query = wiql };

            var result = _workItemStore.QueryByWiqlAsync(w).GetAwaiter().GetResult();
            foreach (var workItem in result.WorkItemRelations) yield return new WorkItemLinkInfoProxy(workItem);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _teamProjectCollection?.Dispose();
                _teamProjectCollection = null;
            }
        }
    }
}