using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Qwiq.Mocks
{
    /// <summary>
    /// Maintains legacy behavior of MockWorkItemStore: WIQL queries return all items, links return all links. Work items are only restricted when querying by ID
    /// </summary>
    public class MockQuery : IQuery
    {
        private readonly string? _wiql;
        private readonly IEnumerable<int>? _ids;
        private readonly MockWorkItemStore _store;

        public MockQuery(
            MockWorkItemStore store,
            string? wiql = null,
            IEnumerable<int>? ids = null)
        {
            _wiql = wiql;
            _ids = ids;
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IWorkItemLinkTypeEndCollection GetLinkTypes()
        {
            return _store.WorkItemLinkTypes.LinkTypeEnds;
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            Trace.TraceInformation("Querying for links " + _wiql ?? "<NONE>");
            return _store.LinkInfo;
        }

        public IWorkItemCollection RunQuery()
        {
            if (_store._lookup == null || _store._lookup.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(MockWorkItemStore)} must be initialized with set of {nameof(IWorkItem)}.");
            }

            if (_ids == null)
            {
                Trace.TraceInformation("Querying for work items " + _wiql ?? "<NONE>");
                return new WorkItemCollection(_store._lookup.Values);
            }

            var h = new HashSet<int>(_ids);
            h.Remove(0);
            var retval = new List<IWorkItem>(h.Count);

            Trace.TraceInformation("Querying for IDs " + string.Join(", ", h));
            foreach (var id in h)
            {
                if (_store._lookup.TryGetValue(id, out IWorkItem? val) && val != null)
                {
                    retval.Add(val);
                }
            }
            return new WorkItemCollection(retval);
        }
    }
}