using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies;
using Microsoft.Qwiq.Proxies.Rest;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    public class QueryProxy : IQuery
    {
        private IEnumerable<int> _ids;

        // This is defined in Microsoft.TeamFoundation.WorkItemTracking.Client.PageSizes
        internal const int MinimumBatchSize = 50;
        internal const int MaximumBatchSize = 200;
        internal const int MaximumFieldSize = 100;

        private readonly Wiql _query;

        private readonly WorkItemTrackingHttpClient _workItemStore;

        private readonly NodeSelect _select;

        private readonly int _batchSize;

        private readonly List<string> _fields;

        private readonly DateTime? AsOf;

        private readonly NodeAndOperator _linkGroup;

        private QueryProxy(
            WorkItemTrackingHttpClient workItemStore,
            NodeSelect select,
            int batchSize = MaximumBatchSize
            )
        {
            _select = select ?? throw new ArgumentNullException(nameof(select));
            _workItemStore = workItemStore ?? throw new ArgumentNullException(nameof(workItemStore));
            // Boundary check the batch size

            if (batchSize < MinimumBatchSize) throw new PageSizeRangeException();
            if (batchSize > MaximumBatchSize) throw new PageSizeRangeException();


            _batchSize = batchSize;

            AsOf = select.GetAsOfUtc();

            NodeAndOperator nodeAndOperator;
            select.GetWhereGroups().TryGetValue(string.Empty, out nodeAndOperator);
            _linkGroup = nodeAndOperator;

            // The API can take up to 100 fields to get with each work item
            // If there are no fields or greater than 100 specified, omit and permit WorkItemExpand to perform the selection
            if (select.Fields != null &&
                select.Fields.Count <= MaximumFieldSize)
            {
                _fields = new List<string>(select.Fields.Count);

                for (var i = 0; i < select.Fields.Count; i++)
                {
                    var field = select.Fields[i];
                    _fields.Add(field.Value);
                }
            }
        }

        internal QueryProxy(
            IEnumerable<int> ids,
            NodeSelect select,
            WorkItemTrackingHttpClient workItemStore,
            int batchSize = MaximumBatchSize
            )
            : this(workItemStore, select, batchSize)
        {
            _ids = ids;
        }

        internal QueryProxy(
            NodeSelect parseResults,
            WorkItemTrackingHttpClient workItemStore,
            int batchSize = MaximumBatchSize)
            : this(workItemStore, parseResults, batchSize)
        {

            _query = new Wiql { Query = parseResults.ToString() };
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            if (_ids == null && _query == null) throw new InvalidOperationException();

            if (_ids == null && _query != null)
            {
                var result = _workItemStore.QueryByWiqlAsync(_query).GetAwaiter().GetResult();
                if (!result.WorkItems.Any()) yield break;
                _ids = result.WorkItems.Select(wir => wir.Id);
            }

            if (_ids == null) yield break;


            var expand = _fields != null ? (WorkItemExpand?)null : WorkItemExpand.Fields;
            var qry = _ids.Partition(_batchSize);
            var ts = qry.Select(s => _workItemStore.GetWorkItemsAsync(s, _fields, AsOf, expand));

            // This is done in parallel so keep performance similar to the SOAP client
            foreach (var workItem in Task.WhenAll(ts).GetAwaiter().GetResult().SelectMany(s => s.Select(f => f)))
            {
                yield return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(workItem));
            }
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            var ends = GetLinkTypes();
            var ends2 = ends as WorkItemLinkTypeEndCollection;
            var useStrong = ends2 != null;
            if (!useStrong)
            {
                ends = new List<IWorkItemLinkTypeEnd>(ends);
            }

            var result = _workItemStore.QueryByWiqlAsync(_query).GetAwaiter().GetResult();
            foreach (var workItemLink in result.WorkItemRelations)
            {
                IWorkItemLinkTypeEnd end = null;

                if (!string.IsNullOrEmpty(workItemLink.Rel))
                {
                    if (useStrong)
                    {
                        ends2.TryGetByName(workItemLink.Rel, out end);
                    }
                    else
                    {
                        end = ends.SingleOrDefault(
                            p => p.ImmutableName.Equals(workItemLink.Rel, StringComparison.OrdinalIgnoreCase));
                    }
                }

                yield return new WorkItemLinkInfoProxy(workItemLink, end?.Id ?? 0);
            }
        }



        public IEnumerable<IWorkItemLinkTypeEnd> GetLinkTypes()
        {
            //TODO: Verify this is a links query
            // if (!IsLinkQuery) return null;

            var wit = WorkItemStoreProxy.GetLinks(_workItemStore);

            // TODO: Limit IWorkItemLinkTypeEnds to the links contained in WIQL


            return wit.LinkTypeEnds;



        }


    }
}