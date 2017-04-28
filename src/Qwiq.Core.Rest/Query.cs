using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
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

        [CanBeNull]
        private HashSet<int> _ids;

        internal Query([NotNull] IEnumerable<int> ids, Wiql query, [NotNull] WorkItemStore workItemStore)
            : this(query, false, workItemStore)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));
            Contract.Requires(workItemStore != null);

            _ids = new HashSet<int>(ids);
        }

        internal Query(Wiql query, bool timePrecision, [NotNull] WorkItemStore workItemStore)

        {
            Contract.Requires(workItemStore != null);

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

        [ItemNotNull]
        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            return RunkLinkQueryImpl().ToList().AsReadOnly();
        }

        public IWorkItemCollection RunQuery()
        {
            if (_ids == null && _query == null) throw new InvalidOperationException();

            // Allocate for method iterator and WorkItemCollection object
            return new WorkItemCollection(RunQueryImpl().ToList());
        }

        private static DateTime? ExtractAsOf(string wiql)
        {
            var m = AsOfRegex.Match(wiql);
            if (!m.Success) return null;

            if (!DateTime.TryParse(m.Groups["date"].Value, out DateTime retval)) throw new Exception();

            return retval;
        }

        private IWorkItemLinkType LinkFunc(string s)
        {
            return _workItemStore.WorkItemLinkTypes[s];
        }

        private IEnumerable<IWorkItemLinkInfo> RunkLinkQueryImpl()
        {
            // Eager loading for the link type ID (which is not returned by the REST API) causes ~250ms delay

            // REVIEW: Closure variable "ends" allocates, preventing local cache
            // REVIEW: Delegate for ctor of Lazy also allocates
            var ends = new Lazy<WorkItemLinkTypeEndCollection>(WorkItemLinkTypeEndValueFactory);
            var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();

            // To avoid an enumerator allocation we are forcing the cast
            for (var index = 0; index < ((List<WorkItemLink>)result.WorkItemRelations).Count; index++)
            {
                // REVIEW: Closure allocation: workItemLink + ends outer closure
                var workItemLink = ((List<WorkItemLink>)result.WorkItemRelations)[index];

                IWorkItemLinkTypeEnd EndValueFactory()
                {
                    return ends.Value.TryGetByName(workItemLink.Rel, out IWorkItemLinkTypeEnd end) ? end : null;
                }

                yield return new WorkItemLinkInfo(
                                                  workItemLink.Source?.Id ?? 0,
                                                  workItemLink.Target?.Id ?? 0,
                                                  new Lazy<IWorkItemLinkTypeEnd>(EndValueFactory));
            }
        }

        [NotNull]
        private IEnumerable<IWorkItem> RunQueryImpl()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IWorkItem>>() != null);

            if (_ids == null && _query != null)
            {
                var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();
                if (!result.WorkItems.Any()) yield break;
                _ids = new HashSet<int>();
                // REVIEW: Possible iterator allocation
                foreach (var wir in result.WorkItems)
                {
                    _ids.Add(wir.Id);
                }
            }

            if (_ids == null) yield break;

            const WorkItemExpand Expand = WorkItemExpand.All;
            var qry = _ids.Partition(_workItemStore.PageSize);
            var ts = qry.Select(
                                s => _workItemStore.NativeWorkItemStore.Value.GetWorkItemsAsync(
                                                                                                s,
                                                                                                null,
                                                                                                _asOf,
                                                                                                Expand,
                                                                                                WorkItemErrorPolicy.Omit));

            // This is done in parallel so keep performance similar to the SOAP client
            foreach (var workItemsPartition in Task.WhenAll(ts).GetAwaiter().GetResult())
            {
                // REIVEW: Allocate for workItem variable
                foreach (var workItem in workItemsPartition)
                {
                    IWorkItemType WorkItemTypeFactory()
                    {
                        // REST API does not return the WIT with the item
                        // Eagerly loading requires several trips to the server at a cost of 50-2500ms for each trip
                        var proj = (string)workItem.Fields[CoreFieldRefNames.TeamProject];
                        var witName = (string)workItem.Fields[CoreFieldRefNames.WorkItemType];
                        return _workItemStore.Projects[proj].WorkItemTypes[witName];
                    }

                    // REIVEW: Allocate for WorkItem reference type
                    yield return new WorkItem(
                                              workItem,
                                              // REVIEW: Allocate for reference type
                                              new Lazy<IWorkItemType>(WorkItemTypeFactory),
                                              // REVIEW: Delegate allocation from method group
                                              LinkFunc).AsProxy();
                }
            }
        }

        private WorkItemLinkTypeEndCollection WorkItemLinkTypeEndValueFactory()
        {
            return (WorkItemLinkTypeEndCollection)GetLinkTypes();
        }
    }
}