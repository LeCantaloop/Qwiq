using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Proxies.Rest;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    public static class NodeSelectExtensions
    {
        /// <summary>
        /// Gets the <see cref="NodeSelect.AsOf"/> as <see cref="DateTime"/> in UTC.
        /// </summary>
        /// <param name="nodeSelect"></param>
        /// <returns>If value is not already in UTC <see langword="null" /> is returned.</returns>
        public static DateTime? GetAsOfUtc(this NodeSelect nodeSelect)
        {
            if (nodeSelect.AsOf == null ||
                !DateTime.TryParse(((NodeItem)nodeSelect.AsOf).Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result))
                return null;
            if (result.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeSelect.AsOf), "Specified date must be UTC");
            }
            return result;
        }
    }

    public class QueryProxy : IQuery
    {
        private IEnumerable<int> _ids;

        // This is defined in Microsoft.TeamFoundation.WorkItemTracking.Client.PageSizes
        internal const int MinimumBatchSize = 50;
        internal const int MaximumBatchSize = 200;
        internal const int MaximumFieldSize = 100;

        private readonly Wiql _query;

        private readonly WorkItemTrackingHttpClient _workItemStore;

        private readonly int _batchSize;

        private readonly List<string> _fields;

        private readonly DateTime? AsOf;

        private QueryProxy(
            WorkItemTrackingHttpClient workItemStore,
            NodeSelect parseResults,
            int batchSize = MaximumBatchSize
            )
        {
            if (parseResults == null) throw new ArgumentNullException(nameof(parseResults));
            _workItemStore = workItemStore ?? throw new ArgumentNullException(nameof(workItemStore));
            // Boundary check the batch size

            if (batchSize < MinimumBatchSize) throw new PageSizeRangeException();
            if (batchSize > MaximumBatchSize) throw new PageSizeRangeException();

            _batchSize = batchSize;

            var asOf = parseResults.GetAsOfUtc();
            if (asOf != DateTime.MinValue)
            {
                AsOf = asOf;
            }

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

        internal QueryProxy(
            IEnumerable<int> ids,
            NodeSelect parseResults,
            WorkItemTrackingHttpClient workItemStore,
            int batchSize = MaximumBatchSize
            )
            : this(workItemStore, parseResults, batchSize)
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
            var result = _workItemStore.QueryByWiqlAsync(_query).GetAwaiter().GetResult();
            foreach (var workItemLink in result.WorkItemRelations)
            {
                yield return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkInfo>(new WorkItemLinkInfoProxy(workItemLink));
            }
        }
    }
}