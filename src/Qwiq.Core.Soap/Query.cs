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

        public IWorkItemLinkTypeEndCollection GetLinkTypes()
        {
            return new WorkItemLinkTypeEndCollection(
                _query
                    .GetLinkTypes()
                    .Select(item => new WorkItemLinkTypeEnd(item))
                    .ToList());
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            // REVIEW: Create an IWorkItemLinkInfo like IWorkItemLinkTypeEndCollection and IWorkItemCollection
            var ends = new Lazy<WorkItemLinkTypeEndCollection>(() => new WorkItemLinkTypeEndCollection(GetLinkTypes()));

            return _query.RunLinkQuery()
                         .Select(
                                 item =>
                                     {
                                         IWorkItemLinkTypeEnd LinkTypeEndFactory() => ends.Value.TryGetById(item.LinkTypeId, out IWorkItemLinkTypeEnd end) ? end : null;

                                         return new WorkItemLinkInfo(item.SourceId, item.TargetId, new Lazy<IWorkItemLinkTypeEnd>(LinkTypeEndFactory));
                                     })
                        .ToList()
                        .AsReadOnly();
        }

        public IWorkItemCollection RunQuery()
        {
            var wic = _query.RunQuery();
            wic.PageSize = _pageSize;

            return
                    wic
                        .Cast<TeamFoundation.WorkItemTracking.Client.WorkItem>()
                        .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>((WorkItem)item))
                        .ToList()
                        .ToWorkItemCollection();
        }
    }
}