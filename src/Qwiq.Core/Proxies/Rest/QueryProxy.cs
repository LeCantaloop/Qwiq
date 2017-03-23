using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Rest;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    public class QueryProxy : IQuery
    {
        private readonly Wiql _query;

        private readonly WorkItemTrackingHttpClient _workItemStore;

        private readonly int _batchSize;

        private readonly List<string> _fields;

        internal QueryProxy(
            NodeSelect parseResults,
            Wiql query,
            WorkItemTrackingHttpClient workItemStore,
            int batchSize = 200)
        {
            _query = query;
            _workItemStore = workItemStore;

            // Boundary check the batch size
            // This is defined in Microsoft.TeamFoundation.WorkItemTracking.Client.PageSizes
            if (batchSize < 50) throw new PageSizeRangeException();
            if (batchSize > 200) throw new PageSizeRangeException();

            _batchSize = batchSize;

            // The API can take up to 100 fields to get with each work item
            // If there are no fields or greater than 100 specified, omit and permit WorkItemExpand to perform the selection
            if (parseResults.Fields != null &&
                parseResults.Fields.Count <= 100)
            {
                _fields = new List<string>(parseResults.Fields.Count);

                for (var i = 0; i < parseResults.Fields.Count; i++)
                {
                    var field = parseResults.Fields[i];
                    _fields.Add(field.Value);
                }
            }
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            var result = _workItemStore.QueryByWiqlAsync(_query).GetAwaiter().GetResult();
            if (result.WorkItems.Any())
            {
                var skip = 0;
                List<WorkItemReference> workItemRefs;
                do
                {
                    Debug.Print("Skipping {0}; Taking {1}", skip, _batchSize);
                    workItemRefs = result.WorkItems.Skip(skip).Take(_batchSize).ToList();
                    Debug.Print("Took {0}", workItemRefs.Count);

                    if (workItemRefs.Any())
                    {
                        // TODO: Support AsOf
                        var workItems = _workItemStore
                            .GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id), _fields, null, _fields != null ? (WorkItemExpand?)null : WorkItemExpand.Fields)
                            .GetAwaiter()
                            .GetResult();
                        foreach (var workItem in workItems)
                        {
                            yield return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(workItem));
                        }
                    }
                    skip += _batchSize;
                }
                while (workItemRefs.Count == _batchSize);
            }
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            var result = _workItemStore.QueryByWiqlAsync(_query).GetAwaiter().GetResult();
            foreach (var workItemLink in result.WorkItemRelations)
            {
                yield return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkInfo>(new WorkItemLinkInfoProxy(workItemLink));
            }
        }
    }
}