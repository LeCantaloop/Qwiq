using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Rest;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    public class QueryProxy : IQuery
    {
        // This is defined in Microsoft.TeamFoundation.WorkItemTracking.Client.PageSizes
        internal const int MinimumBatchSize = 50;
        internal const int MaximumBatchSize = 200;
        internal const int MaximumFieldSize = 100;

        private readonly Wiql _query;

        private readonly WorkItemTrackingHttpClient _workItemStore;

        private readonly int _batchSize;

        private readonly List<string> _fields;

        internal QueryProxy(
            NodeSelect parseResults,
            Wiql query,
            WorkItemTrackingHttpClient workItemStore,
            int batchSize = MaximumBatchSize)
        {
            _query = query;
            _workItemStore = workItemStore;

            // Boundary check the batch size

            if (batchSize < MinimumBatchSize) throw new PageSizeRangeException();
            if (batchSize > MaximumBatchSize) throw new PageSizeRangeException();

            _batchSize = batchSize;

            // The API can take up to 100 fields to get with each work item
            // If there are no fields or greater than 100 specified, omit and permit WorkItemExpand to perform the selection
            if (parseResults.Fields != null &&
                parseResults.Fields.Count <= MaximumFieldSize)
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
            if (!result.WorkItems.Any()) yield break;

            // This is done in parallel so keep performance similar to the SOAP client
            var expand = _fields != null ? (WorkItemExpand?)null : WorkItemExpand.Fields;
            var qry = result.WorkItems.Partition(_batchSize);
            var ts = qry.Select(s => _workItemStore.GetWorkItemsAsync(s.Select(wir => wir.Id), _fields, null, expand));

            foreach (var workItem in Task.WhenAll(ts).GetAwaiter().GetResult().SelectMany(s => s.Select(f => f)))
            {
                yield return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(workItem));
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