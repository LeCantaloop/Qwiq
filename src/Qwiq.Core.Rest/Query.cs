using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    internal class Query : IQuery
    {
        internal const int MaximumBatchSize = 200;

        internal const int MaximumFieldSize = 100;

        // This is defined in Microsoft.TeamFoundation.WorkItemTracking.Client.PageSizes
        internal const int MinimumBatchSize = 50;

        private static readonly Regex AsOfRegex = new Regex(
                                                            @"(?<operand>asof\s)\'(?<date>.*)\'",
                                                            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly DateTime? _asOf;

        private readonly Wiql _query;

        private readonly bool _timePrecision;

        private readonly WorkItemStore _workItemStore;

        private IEnumerable<int> _ids;

        internal Query(IEnumerable<int> ids, Wiql query, WorkItemStore workItemStore)
            : this(query, false, workItemStore)
        {
            _ids = ids;
        }

        internal Query(Wiql query, bool timePrecision, WorkItemStore workItemStore)

        {
            _workItemStore = workItemStore ?? throw new ArgumentNullException(nameof(workItemStore));
            _timePrecision = timePrecision;
            _query = query;
            _asOf = ExtractAsOf(query.Query);
        }

        public IWorkItemLinkTypeEndCollection GetLinkTypes()
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
                                                  workItemLink.Source?.Id ?? 0,
                                                  workItemLink.Target?.Id ?? 0,
                                                  new Lazy<IWorkItemLinkTypeEnd>(
                                                                                 () => ends.Value.TryGetByName(
                                                                                                               workItemLink.Rel,
                                                                                                               out IWorkItemLinkTypeEnd end)
                                                                                           ? end
                                                                                           : null));
            }
        }

        public IWorkItemCollection RunQuery()
        {
            if (_ids == null && _query == null) throw new InvalidOperationException();

            return new WorkItemCollection(RunQueryImpl().ToList());
        }

        private IEnumerable<IWorkItem> RunQueryImpl()
        {
            if (_ids == null && _query != null)
            {
                var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();
                if (!result.WorkItems.Any()) yield break;
                _ids = result.WorkItems.Select(wir => wir.Id);
            }

            if (_ids == null) yield break;

            var expand = WorkItemExpand.All;
            var qry = _ids.Partition(_workItemStore.PageSize);
            var ts = qry.Select(s => _workItemStore.NativeWorkItemStore.Value.GetWorkItemsAsync(s, null, _asOf, expand, WorkItemErrorPolicy.Omit));

            // This is done in parallel so keep performance similar to the SOAP client
            foreach (var workItem in Task.WhenAll(ts).GetAwaiter().GetResult().SelectMany(s => s.Select(f => f)))
            {
                IWorkItemType WorkItemTypeFactory()
                {
                    // REST API does not return the WIT with the item
                    // Eagerly loading requires several trips to the server at a cost of 50-2500ms for each trip
                    var proj = (string)workItem.Fields[CoreFieldRefNames.TeamProject];
                    var witName = (string)workItem.Fields[CoreFieldRefNames.WorkItemType];
                    return _workItemStore.Projects[proj].WorkItemTypes[witName];
                }

                yield return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(
                                                                                    new WorkItem(
                                                                                                 workItem,
                                                                                                 new Lazy<IWorkItemType>(WorkItemTypeFactory),
                                                                                                 s => _workItemStore.WorkItemLinkTypes[s]));
            }
        }

        private static DateTime? ExtractAsOf(string wiql)
        {
            var m = AsOfRegex.Match(wiql);
            if (!m.Success) return null;

            if (!DateTime.TryParse(m.Groups["date"].Value, out DateTime retval)) throw new Exception();

            return retval;
        }
    }
}