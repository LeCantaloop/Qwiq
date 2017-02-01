using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Soap;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{


    public class WorkItemStoreProxy : IWorkItemStore
    {
        private IInternalTfsTeamProjectCollection _teamProjectCollection;

        private readonly WorkItemTrackingHttpClient _workItemStore;

        

        internal WorkItemStoreProxy(
            IInternalTfsTeamProjectCollection teamProjectCollection,
            WorkItemTrackingHttpClient workItemStore)
        {
            if (teamProjectCollection == null) throw new ArgumentNullException(nameof(teamProjectCollection));
            if (workItemStore == null) throw new ArgumentNullException(nameof(workItemStore));            

            _teamProjectCollection = teamProjectCollection;
            _workItemStore = workItemStore;            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _teamProjectCollection?.Dispose();
                _teamProjectCollection = null;
            }
        }

        public IEnumerable<IWorkItem> Query(string wiql, bool dayPrecision = false)
        {
            if (dayPrecision) throw new NotSupportedException();

            var p = Parser.ParseSyntax(wiql);
            var w = new Wiql() { Query = p.ToString() };
            
            var fields = new List<string>();
            for (var i = 0; i < p.Fields.Count; i++)
            {
                var field = p.Fields[i];
                fields.Add(field.Value);
            }


            var result = _workItemStore.QueryByWiqlAsync(w, "OS").GetAwaiter().GetResult();
            if (result.WorkItems.Any())
            {
                int skip = 0;
                const int BatchSize = 100;
                IEnumerable<WorkItemReference> workItemRefs;
                do
                {
                    workItemRefs = result.WorkItems.Skip(skip).Take(BatchSize);
                    if (workItemRefs.Any())
                    {
                        // TODO: Support AsOf
                        var workItems = _workItemStore.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id), fields,null, null).GetAwaiter().GetResult();
                        foreach (var workItem in workItems)
                        {
                            yield return new WorkItemProxy(workItem);
                        }
                    }
                    skip += BatchSize;
                }
                while (workItemRefs.Count() == BatchSize);
            }
        }
        private static IEnumerable<int> Ids(WorkItemQueryResult result, int skip = 0)
        {
            return result.WorkItemRelations.Where(r => r.Target != null).Select(r => r.Target.Id)
                .Union(result.WorkItemRelations.Where(r => r.Source != null).Select(r => r.Source.Id))
                .Skip(skip)
                .Take(100);
        }

        public IEnumerable<IWorkItemLinkInfo> QueryLinks(string wiql, bool dayPrecision = false)
        {
            if (dayPrecision) throw new NotSupportedException();

            var w = new Wiql() { Query = wiql };

            var result = _workItemStore.QueryByWiqlAsync(w, "OS").GetAwaiter().GetResult();
            if (result.WorkItemRelations.Any())
            {
                int skip = 0;
                const int BatchSize = 200;
                IEnumerable<WorkItemLink> workItemRefs;
                do
                {
                    workItemRefs = result.WorkItemRelations.Skip(skip).Take(BatchSize);
                    if (workItemRefs.Any())
                    {

                        foreach (var workItem in workItemRefs)
                        {
                            // write work item to console
                            yield return new WorkItemLinkInfoProxy(workItem);
                        }
                    }
                    skip += BatchSize;
                }
                while (workItemRefs.Count() == BatchSize);
            }
        }

        public IEnumerable<IWorkItem> Query(IEnumerable<int> ids, DateTime? asOf = null)
        {
            if (!ids.Any())
            {
                yield return null;
            }

            var wis = _workItemStore.GetWorkItemsAsync(ids, null, asOf, WorkItemExpand.Fields).GetAwaiter().GetResult();
            foreach (var workItem in wis)
            {
                // write work item to console
                yield return new WorkItemProxy(workItem);
            }
        }

        public IWorkItem Query(int id, DateTime? asOf = null)
        {
            var wi = _workItemStore.GetWorkItemAsync(id, null, asOf, WorkItemExpand.Fields).GetAwaiter().GetResult();
            return new WorkItemProxy(wi);
        }

        public ITfsTeamProjectCollection TeamProjectCollection => _teamProjectCollection;

        public IEnumerable<IProject> Projects
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IWorkItemLinkType> WorkItemLinkTypes { get; }

        public TimeZone TimeZone
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
