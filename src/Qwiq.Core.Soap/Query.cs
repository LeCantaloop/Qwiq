using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Common;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class Query : IQuery
    {
        private readonly int _pageSize;

        private readonly TeamFoundation.WorkItemTracking.Client.Query _query;

        internal Query([NotNull] TeamFoundation.WorkItemTracking.Client.Query query, int pageSize = PageSizeLimits.DefaultPageSize)
        {
            Contract.Requires(query != null);
            Contract.Requires(pageSize < PageSizeLimits.MaxPageSize);
            Contract.Requires(pageSize > PageSizeLimits.DefaultPageSize);

            _pageSize = pageSize;
            _query = query ?? throw new ArgumentNullException(nameof(query));

            if (pageSize < PageSizeLimits.DefaultPageSize || pageSize > PageSizeLimits.MaxPageSize) throw new PageSizeRangeException();
        }

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


            return _query.RunLinkQuery()
                         .Select(
                                 item =>
                                     {
                                         IWorkItemLinkTypeEnd LinkTypeEndFactory() => GetLinkTypes().TryGetById(item.LinkTypeId, out IWorkItemLinkTypeEnd end) ? end : null;

                                         return new WorkItemLinkInfo(item.SourceId, item.TargetId, new Lazy<IWorkItemLinkTypeEnd>(LinkTypeEndFactory));
                                     })
                        .ToList()
                        .AsReadOnly();
        }

        public IWorkItemCollection RunQuery()
        {
            var wic = _query.RunQuery();
            wic.PageSize = _pageSize;

            var items = new List<IWorkItem>(wic.Count);
            for (var i = 0; i < wic.Count; i++)
            {
                var item = ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>((WorkItem)wic[i]);
                items.Add(item);
            }

            return new WorkItemCollection(items);
        }
    }
}