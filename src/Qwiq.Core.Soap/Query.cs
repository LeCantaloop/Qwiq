using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Common;

namespace Microsoft.Qwiq.Soap
{
    internal class Query : IQuery
    {
        private readonly int _pageSize;

        private readonly TeamFoundation.WorkItemTracking.Client.Query _query;

        internal Query(TeamFoundation.WorkItemTracking.Client.Query query, int pageSize = PageSizeLimits.DefaultPageSize)
        {
            _pageSize = pageSize;
            _query = query ?? throw new ArgumentNullException(nameof(query));

            if (pageSize < PageSizeLimits.DefaultPageSize || pageSize > PageSizeLimits.MaxPageSize)
                throw new PageSizeRangeException();
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            var wic = _query.RunQuery();
            wic.PageSize = _pageSize;

            return wic.Cast<TeamFoundation.WorkItemTracking.Client.WorkItem>()
                      .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>((WorkItem)item));
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            return _query.RunLinkQuery()
                         .Select(item => new WorkItemLinkInfo(item));
        }

        public IEnumerable<IWorkItemLinkTypeEnd> GetLinkTypes()
        {
            return _query.GetLinkTypes()
                         .Select(item => new WorkItemLinkTypeEnd(item));
        }
    }
}
