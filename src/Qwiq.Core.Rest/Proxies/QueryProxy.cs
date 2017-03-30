using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    public class QueryProxy : IQuery
    {
        internal const int MaximumBatchSize = 200;

        internal const int MaximumFieldSize = 100;

        // This is defined in Microsoft.TeamFoundation.WorkItemTracking.Client.PageSizes
        internal const int MinimumBatchSize = 50;

        private readonly List<string> _fields;

        private readonly bool _timePrecision;

        private readonly Wiql _query;

        private readonly WorkItemStoreProxy _workItemStore;

        private readonly DateTime? _asOf;

        private IEnumerable<int> _ids;

        private static readonly Regex AsOfRegex = new Regex(@"(?<operand>asof\s)\'(?<date>.*)\'", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        internal QueryProxy(
            IEnumerable<int> ids,
            Wiql query,
            WorkItemStoreProxy workItemStore)
            : this(query, false, workItemStore)
        {
            _ids = ids;
        }

        internal QueryProxy(
            Wiql query,
            bool timePrecision,
            WorkItemStoreProxy workItemStore)

        {
            _workItemStore = workItemStore ?? throw new ArgumentNullException(nameof(workItemStore));
            _timePrecision = timePrecision;
            _query = query;
            _asOf = ExtractAsOf(query.Query);
        }
        private static DateTime? ExtractAsOf(string wiql)
        {
            var m = AsOfRegex.Match(wiql);
            if (!m.Success) return null;

            DateTime retval;
            if (!DateTime.TryParse(m.Value, out retval))
            {
                throw new Exception();
            }

            return retval;
        }


        public IEnumerable<IWorkItemLinkTypeEnd> GetLinkTypes()
        {
            //TODO: Verify this is a links query
            // if (!IsLinkQuery) return null;

            var wit = _workItemStore.WorkItemLinkTypes;

            // TODO: Limit IWorkItemLinkTypeEnds to the links contained in WIQL

            return wit.LinkTypeEnds;
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            // Eager loading for the link type ID (which is not returned by the REST API) causes ~250ms delay
            var ends = new Lazy<WorkItemLinkTypeEndCollection>(() => (WorkItemLinkTypeEndCollection)GetLinkTypes());
            var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();

            foreach (var workItemLink in result.WorkItemRelations)
            {
                yield return new WorkItemLinkInfo(
                    workItemLink,
                    new Lazy<int>(() => ends.Value.TryGetByName(workItemLink.Rel, out IWorkItemLinkTypeEnd end) ? end.Id : 0));
            }
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            if (_ids == null && _query == null) throw new InvalidOperationException();

            if (_ids == null && _query != null)
            {
                var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();
                if (!result.WorkItems.Any()) yield break;
                _ids = result.WorkItems.Select(wir => wir.Id);
            }

            if (_ids == null) yield break;

            var expand = _fields != null ? (WorkItemExpand?)null : WorkItemExpand.Fields;
            var qry = _ids.Partition(_workItemStore.BatchSize);
            var ts = qry.Select(s => _workItemStore.NativeWorkItemStore.Value.GetWorkItemsAsync(s, _fields, _asOf, expand, WorkItemErrorPolicy.Omit));

            // REST API does not return the WIT with the item
            // Eagerly loading requires several trips to the server at a cost of 50-250ms for each project

            // REVIEW: Can we only request the projects needed instead of all?
            var wits = new Lazy<Dictionary<string, Dictionary<string, IWorkItemType>>>(() => _workItemStore.Projects.ToDictionary(
                k => k.Name,
                e => e.WorkItemTypes.ToDictionary(
                    i => i.Name,
                    j => j,
                    StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase));

            // This is done in parallel so keep performance similar to the SOAP client
            foreach (var workItem in Task.WhenAll(ts).GetAwaiter().GetResult().SelectMany(s => s.Select(f => f)))
            {
                yield return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(workItem, new Lazy<IWorkItemType>(
                                                                                                          () =>
                                                                                                              {
                                                                                                                  var proj = wits.Value[(string)workItem.Fields[CoreFieldRefNames.TeamProject]];
                                                                                                                  var wit = proj[(string)workItem.Fields[CoreFieldRefNames.WorkItemType]];
                                                                                                                  return wit;
                                                                                                              })));
            }
        }
    }
}