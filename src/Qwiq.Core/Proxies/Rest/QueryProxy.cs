using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Qwiq.Exceptions;
using Microsoft.Qwiq.Rest;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Constants;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class QueryProxy : IQuery
    {
        internal const int MaximumBatchSize = 200;

        internal const int MaximumFieldSize = 100;

        // This is defined in Microsoft.TeamFoundation.WorkItemTracking.Client.PageSizes
        internal const int MinimumBatchSize = 50;

        private readonly List<string> _fields;

        private readonly NodeAndOperator _linkGroup;

        private readonly Wiql _query;

        private readonly NodeSelect _select;

        private readonly WorkItemStoreProxy _workItemStore;

        private readonly DateTime? AsOf;

        private IEnumerable<int> _ids;

        internal QueryProxy(
            IEnumerable<int> ids,
            NodeSelect select,
            WorkItemStoreProxy workItemStore)
            : this(workItemStore, select)
        {
            _ids = ids;
        }

        internal QueryProxy(
            NodeSelect parseResults,
            WorkItemStoreProxy workItemStore)
            : this(workItemStore, parseResults)
        {
            _query = new Wiql { Query = parseResults.ToString() };
        }

        private QueryProxy(
            WorkItemStoreProxy workItemStore,
            NodeSelect select)
        {
            _select = select ?? throw new ArgumentNullException(nameof(select));
            _workItemStore = workItemStore ?? throw new ArgumentNullException(nameof(workItemStore));

            AsOf = select.GetAsOfUtc();

            NodeAndOperator nodeAndOperator;
            select.GetWhereGroups().TryGetValue(string.Empty, out nodeAndOperator);
            _linkGroup = nodeAndOperator;

            // The API can take up to 100 fields to get with each work item
            // If there are no fields or greater than 100 specified, omit and permit WorkItemExpand to perform the selection
            if (select.Fields != null && select.Fields.Count <= MaximumFieldSize)
            {
                _fields = new List<string>(select.Fields.Count);

                for (var i = 0; i < select.Fields.Count; i++)
                {
                    var field = select.Fields[i];
                    _fields.Add(field.Value);
                }
            }
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
            var ends = new Lazy<WorkItemLinkTypeEndCollection>(()=>(WorkItemLinkTypeEndCollection)GetLinkTypes());
            var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query).GetAwaiter().GetResult();

            foreach (var workItemLink in result.WorkItemRelations)
            {
                yield return new WorkItemLinkInfoProxy(
                    workItemLink,
                    new Lazy<int>(()=> ends.Value.TryGetByName(workItemLink.Rel, out IWorkItemLinkTypeEnd end) ? end.Id : 0));
            }
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            if (_ids == null && _query == null) throw new InvalidOperationException();

            if (_ids == null && _query != null)
            {
                var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query).GetAwaiter().GetResult();
                if (!result.WorkItems.Any()) yield break;
                _ids = result.WorkItems.Select(wir => wir.Id);
            }

            if (_ids == null) yield break;

            var expand = _fields != null ? (WorkItemExpand?)null : WorkItemExpand.Fields;
            var qry = _ids.Partition(_workItemStore.BatchSize);
            var ts = qry.Select(s => _workItemStore.NativeWorkItemStore.Value.GetWorkItemsAsync(s, _fields, AsOf, expand));

            // REST API does not return the WIT with the item
            // Eagerly loading requires several trips to the server at a cost of 50-250ms for each project

            // REVIEW: Can we only request the projects needed instead of all?
            var wits = new Lazy<Dictionary<string, Dictionary<string, IWorkItemType>>>(()=> _workItemStore.Projects.ToDictionary(
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