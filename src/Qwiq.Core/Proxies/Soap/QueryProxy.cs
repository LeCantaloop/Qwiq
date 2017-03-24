using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class QueryProxy : IQuery
    {
        private readonly TeamFoundation.WorkItemTracking.Client.Query _query;

        internal QueryProxy(TeamFoundation.WorkItemTracking.Client.Query query)
        {
            _query = query;
        }

        public IEnumerable<IWorkItem> RunQuery()
        {
            return _query.RunQuery().Cast<TeamFoundation.WorkItemTracking.Client.WorkItem>().Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(item)));
        }

        public IEnumerable<IWorkItemLinkInfo> RunLinkQuery()
        {
            return _query.RunLinkQuery().Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkInfo>(new WorkItemLinkInfoProxy(item)));
        }

        public IEnumerable<IWorkItemLinkTypeEnd> GetLinkTypes()
        {
            return _query.GetLinkTypes()
                         .Select(
                             item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(
                                 new WorkItemLinkTypeEndProxy(item)));
        }
    }
}
