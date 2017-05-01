using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // TODO: Make this configurable
        private const WorkItemExpand Expand = WorkItemExpand.All;

        private static readonly Regex AsOfRegex = new Regex(
                                                            @"(?<operand>asof\s)\'(?<date>.*)\'",
                                                            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly ReadOnlyCollection<IWorkItem> EmptyWorkItems = new ReadOnlyCollection<IWorkItem>(new List<IWorkItem>());

        private readonly DateTime? _asOf;

        private readonly Wiql _query;

        private readonly bool _timePrecision;

        [NotNull]
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
            return RunkLinkQueryImpl();
        }

        public IWorkItemCollection RunQuery()
        {
            if (_ids == null && _query == null) throw new InvalidOperationException();

            // Allocate for method iterator and WorkItemCollection object
            return new WorkItemCollection(RunQueryImpl());
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

        private ReadOnlyCollection<IWorkItemLinkInfo> RunkLinkQueryImpl()
        {
            // Eager loading for the link type ID (which is not returned by the REST API) causes ~250ms delay

            // REVIEW: Closure variable "ends" allocates, preventing local cache
            // REVIEW: Delegate for ctor of Lazy also allocates
            var ends = new Lazy<WorkItemLinkTypeEndCollection>(WorkItemLinkTypeEndValueFactory);
            var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();

            // To avoid an enumerator allocation we are forcing the cast
            var result2 = (List<WorkItemLink>)result.WorkItemRelations;
            var retval = new List<IWorkItemLinkInfo>(result2.Count);

            for (var index = 0; index < result2.Count; index++)
            {
                // REVIEW: Closure allocation: workItemLink + ends outer closure
                var workItemLink = result2[index];

                IWorkItemLinkTypeEnd EndValueFactory()
                {
                    return ends.Value.TryGetByName(workItemLink.Rel, out IWorkItemLinkTypeEnd end) ? end : null;
                }

                retval.Add(
                           new WorkItemLinkInfo(
                                                workItemLink.Source?.Id ?? 0,
                                                workItemLink.Target?.Id ?? 0,
                                                new Lazy<IWorkItemLinkTypeEnd>(EndValueFactory)));
            }

            return retval.AsReadOnly();
        }

        [NotNull]
        private ReadOnlyCollection<IWorkItem> RunQueryImpl()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IWorkItem>>() != null);

            if (_ids == null && _query != null)
            {
                var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();
                if (!result.WorkItems.Any()) return EmptyWorkItems;
                _ids = new HashSet<int>();
                var items = (List<WorkItemReference>)result.WorkItems;
                for (var i = 0; i < items.Count; i++)
                {
                    var wir = items[i];
                    _ids.Add(wir.Id);
                }
            }

            if (_ids == null) return EmptyWorkItems;

            var retval = new List<IWorkItem>(_ids.Count);

            var ts = new List<Task<List<TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>>>(_ids.Count % _workItemStore.PageSize + 1);
            var qry = _ids.Partition(_workItemStore.PageSize);
            foreach (var s in qry)
                ts.Add(_workItemStore.NativeWorkItemStore.Value.GetWorkItemsAsync(s, null, _asOf, Expand, WorkItemErrorPolicy.Omit));

            var results = Task.WhenAll(ts).GetAwaiter().GetResult();

            // This is done in parallel so keep performance similar to the SOAP client
            for (var i = 0; i < results.Length; i++)
            {
                var workItemsPartition = results[i];
                // REIVEW: Allocate for workItem variable
                for (var j = 0; j < workItemsPartition.Count; j++)
                {
                    var workItem = workItemsPartition[j];

                    // REIVEW: Allocate for WorkItem reference type
                    var wi = new WorkItem(
                                          workItem,
                                          // REVIEW: Allocate for reference type
                                          _workItemStore
                                                  .Projects[(string)workItem.Fields[CoreFieldRefNames.TeamProject]]
                                                  .WorkItemTypes[(string)workItem.Fields[CoreFieldRefNames.WorkItemType]],
                                          // REVIEW: Delegate allocation from method group
                                          LinkFunc).AsProxy();

                    retval.Add(wi);
                }
            }

            return retval.AsReadOnly();
        }

        private WorkItemLinkTypeEndCollection WorkItemLinkTypeEndValueFactory()
        {
            return (WorkItemLinkTypeEndCollection)GetLinkTypes();
        }
    }
}