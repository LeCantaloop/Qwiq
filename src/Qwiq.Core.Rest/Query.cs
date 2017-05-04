using JetBrains.Annotations;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class Query : IQuery
    {
        private static readonly Regex AsOfRegex = new Regex(
                                                            @"(?<operand>asof\s)\'(?<date>.*)\'",
                                                            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly List<IWorkItem> EmptyWorkItems = new List<IWorkItem>(0);

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
            // REVIEW: Create an IWorkItemLinkInfo like IWorkItemLinkTypeEndCollection and IWorkItemCollection
            return _workItemStore.Configuration.LazyLoadingEnabled ? RunkLinkQueryImplLazy() : RunkLinkQueryImpl();
        }

        public IWorkItemCollection RunQuery()
        {
            if (_ids == null && _query == null) throw new InvalidOperationException();

            // Allocate for method iterator and WorkItemCollection object
            return new WorkItemCollection(_workItemStore.Configuration.LazyLoadingEnabled ? RunQueryImplLazy() : RunQueryImpl());
        }

        private static DateTime? ExtractAsOf(string wiql)
        {
            var m = AsOfRegex.Match(wiql);
            if (!m.Success) return null;

            if (!DateTime.TryParse(m.Groups["date"].Value, out DateTime retval)) throw new Exception();

            return retval;
        }

        private WorkItem CreateItemEager(TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
        {
            return new WorkItem(
                                workItem,
                                // REVIEW: Allocate for reference type
                                _workItemStore.Projects[(string)workItem.Fields[CoreFieldRefNames.TeamProject]]
                                              .WorkItemTypes[(string)workItem.Fields[CoreFieldRefNames.WorkItemType]],
                                // REVIEW: Delegate allocation from method group
                                LinkFunc);
        }

        private WorkItem CreateItemLazy(TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
        {
            IWorkItemType WorkItemTypeFactory()
            {
                return _workItemStore.Projects[(string)workItem.Fields[CoreFieldRefNames.TeamProject]]
                                     .WorkItemTypes[(string)workItem.Fields[CoreFieldRefNames.WorkItemType]];
            }

            return new WorkItem(workItem, new Lazy<IWorkItemType>(WorkItemTypeFactory), LinkFunc);
        }

        private List<TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>[] FetchResults()
        {
            var t = new CancellationToken();
            var ts =
                    new List<Task<List<TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>>>(
                                                                                                 _ids.Count
                                                                                                 % _workItemStore.Configuration.PageSize
                                                                                                 + 1);
            var qry = _ids.Partition(_workItemStore.Configuration.PageSize);

            var c = _workItemStore.NativeWorkItemStore.Value;
            var o = _workItemStore.Configuration;

            var e = (TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemExpand)o.WorkItemExpand;
            var p = (TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy)o.WorkItemErrorPolicy;
            var f = o.WorkItemExpand == WorkItemExpand.None ? o.DefaultFields : null;

            foreach (var s in qry) ts.Add(c.GetWorkItemsAsync(s, f, _asOf, e, p, null, t));
            // This is done in parallel so keep performance similar to the SOAP client
            var results = Task.WhenAll(ts.ToArray()).ConfigureAwait(false).GetAwaiter().GetResult();
            return results;
        }

        private IWorkItemLinkType LinkFunc(string s)
        {
            return _workItemStore.WorkItemLinkTypes[s];
        }

        private List<IWorkItem> LoadWorkItemsEagerly(List<TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>[] results)
        {
            var retval = new List<IWorkItem>(_ids.Count);

            // This is done in parallel so keep performance similar to the SOAP client
            for (var i = 0; i < results.Length; i++)
            {
                var workItemsPartition = results[i];
                // REIVEW: Allocate for workItem variable
                for (var j = 0; j < workItemsPartition.Count; j++)
                {
                    var workItem = workItemsPartition[j];

                    var wi = CreateItemEager(workItem);

                    retval.Add(_workItemStore.Configuration.ProxyCreationEnabled ? wi.AsProxy() : wi);
                }
            }

            return retval;
        }

        private ReadOnlyCollection<IWorkItemLinkInfo> RunkLinkQueryImpl()
        {
            // Eager loading for the link type ID (which is not returned by the REST API) causes ~250ms delay
            var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();

            // To avoid an enumerator allocation we are forcing the cast
            var result2 = (List<WorkItemLink>)result.WorkItemRelations;
            var retval = new List<IWorkItemLinkInfo>(result2.Count + 1);
            var ends = GetLinkTypes();

            for (var index = 0; index < result2.Count; index++)
            {
                ends.TryGetByName(result2[index].Rel, out IWorkItemLinkTypeEnd end);
                retval.Add(new WorkItemLinkInfo(result2[index].Source?.Id ?? 0, result2[index].Target?.Id ?? 0, end));
            }

            return retval.AsReadOnly();
        }

        private IEnumerable<IWorkItemLinkInfo> RunkLinkQueryImplLazy()
        {
            // Eager loading for the link type ID (which is not returned by the REST API) causes ~250ms delay

            // REVIEW: Closure variable "ends" allocates, preventing local cache
            // REVIEW: Delegate for ctor of Lazy also allocates
            var ends = new Lazy<WorkItemLinkTypeEndCollection>(WorkItemLinkTypeEndValueFactory);
            var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();

            // To avoid an enumerator allocation we are forcing the cast
            var result2 = (List<WorkItemLink>)result.WorkItemRelations;

            for (var i = 0; i < result2.Count; i++)
            {
                WorkItemLink t = result2[i];

                // REVIEW: Closure allocation: workItemLink + ends outer closure
                IWorkItemLinkTypeEnd EndValueFactory()
                {
                    return ends.Value.TryGetByName(t.Rel, out IWorkItemLinkTypeEnd end) ? end : null;
                }

                var ltEnd = new Lazy<IWorkItemLinkTypeEnd>(EndValueFactory);

                yield return new WorkItemLinkInfo(t.Source?.Id ?? 0, t.Target?.Id ?? 0, ltEnd);
            }
        }

        [NotNull]
        private List<IWorkItem> RunQueryImpl()
        {
            Contract.Ensures(Contract.Result<List<IWorkItem>>() != null);

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

            var results = FetchResults();

            return LoadWorkItemsEagerly(results);
        }

        [NotNull]
        private IEnumerable<IWorkItem> RunQueryImplLazy()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IWorkItem>>() != null);

            if (_ids == null && _query != null)
            {
                var result = _workItemStore.NativeWorkItemStore.Value.QueryByWiqlAsync(_query, _timePrecision).GetAwaiter().GetResult();
                if (!result.WorkItems.Any()) yield break;
                _ids = new HashSet<int>();
                var items = (List<WorkItemReference>)result.WorkItems;
                for (var i = 0; i < items.Count; i++)
                {
                    var wir = items[i];
                    _ids.Add(wir.Id);
                }
            }

            if (_ids == null) yield break;

            var results = FetchResults();

            // This is not encapsulated in own method to avoid allocation of iterator
            for (var i = 0; i < results.Length; i++)
            {
                var workItemsPartition = results[i];

                for (var j = 0; j < workItemsPartition.Count; j++)
                {
                    var workItem = workItemsPartition[j];

                    var wi = _workItemStore.Configuration.LazyLoadingEnabled ? CreateItemLazy(workItem) : CreateItemEager(workItem);

                    yield return _workItemStore.Configuration.ProxyCreationEnabled ? wi.AsProxy() : wi;
                }
            }
        }

        private WorkItemLinkTypeEndCollection WorkItemLinkTypeEndValueFactory()
        {
            return (WorkItemLinkTypeEndCollection)GetLinkTypes();
        }
    }
}