using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class Query : IQuery
    {
        private readonly int _pageSize;

        private readonly TeamFoundation.WorkItemTracking.Client.Query _query;

        internal Query([NotNull] TeamFoundation.WorkItemTracking.Client.Query query, int pageSize)
        {
            Contract.Requires(query != null);
            _query = query;
            _pageSize = pageSize;
        }

        [CanBeNull]
        private IWorkItemLinkTypeEndCollection _linkTypes;

        public IWorkItemLinkTypeEndCollection GetLinkTypes()
        {
            if (_linkTypes != null) return _linkTypes;

            var lt = _query.GetLinkTypes();
            var lte = new List<IWorkItemLinkTypeEnd>(lt.Length);
            foreach (var l in lt)
            {
                var item = new WorkItemLinkTypeEnd(l);
                lte.Add(item);
            }

            var retval = new WorkItemLinkTypeEndCollection(lte);
            _linkTypes = retval;

            return _linkTypes;
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            // REVIEW: Create an IWorkItemLinkInfo like IWorkItemLinkTypeEndCollection and IWorkItemCollection
            var wili = _query.RunLinkQuery();
            var retval = new List<IWorkItemLinkInfo>(wili.Length);
            var lt = GetLinkTypes().ToDictionary(k=>k.Id, e=>e);
            for (var i = 0; i < wili.Length; i++)
            {
                if (wili[i].LinkTypeId == 0) continue;

                // TODO: Use Lazy config options


                var lte = lt[wili[i].LinkTypeId];
                retval.Add(new WorkItemLinkInfo(wili[i].SourceId, wili[i].TargetId, lte));
            }

            return retval.AsReadOnly();
        }

        public IWorkItemCollection RunQuery()
        {
            var wic = _query.RunQuery();
            wic.PageSize = _pageSize;

            // TODO: Use Lazy config options
            var items = new List<IWorkItem>(wic.Count);
            for (var i = 0; i < wic.Count; i++)
            {
                // TODO: Use proxy config options
                var item = ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>((WorkItem)wic[i]);
                items.Add(item);
            }

            return new WorkItemCollection(items);
        }
    }
}