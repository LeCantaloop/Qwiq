using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Soap
{
    internal class Query : IQuery
    {
        private readonly TeamFoundation.WorkItemTracking.Client.Query _query;

        internal Query(TeamFoundation.WorkItemTracking.Client.Query query)
        {
            _query = query;
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            return _query.RunQuery()
                         .Cast<TeamFoundation.WorkItemTracking.Client.WorkItem>()
                         .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItem(item)));
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
