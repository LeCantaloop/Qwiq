using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Rest;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Microsoft.Qwiq.Rest
{
    public class QueryProxy : IQuery
    {


        private readonly Wiql _query;

        private readonly WorkItemTrackingHttpClient _workItemStore;

        private readonly int _batchSize;

        private readonly List<string> _fields;

        internal QueryProxy(NodeSelect parseResults, Wiql query, WorkItemTrackingHttpClient workItemStore, int batchSize = 100)
        {
            _query = query;
            _workItemStore = workItemStore;

            // Boundary check the batch size
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, "Batch size must be greater than 0.");
            if (batchSize > 200) throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, "Batch size must be less than 200.");

            _batchSize = batchSize;

            if (parseResults.Fields != null)
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
                IEnumerable<WorkItemReference> workItemRefs;
                do
                {
                    workItemRefs = result.WorkItems.Skip(skip).Take(_batchSize).ToList();
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
                while (workItemRefs.Count() == _batchSize);
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