using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class WorkItemLinkTypeEndProxy : IWorkItemLinkTypeEnd
    {
        private readonly Tfs.WorkItemLinkTypeEnd _end;

        internal WorkItemLinkTypeEndProxy(Tfs.WorkItemLinkTypeEnd end)
        {
            _end = end;
        }

        public int Id
        {
            get { return _end.Id; }
        }

        public string ImmutableName
        {
            get { return _end.ImmutableName; }
        }

        public bool IsForwardLink
        {
            get { return _end.IsForwardLink; }
        }

        public IWorkItemLinkType LinkType
        {
            get { return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkType>(new WorkItemLinkTypeProxy(_end.LinkType)); }
        }

        public string Name
        {
            get { return _end.Name; }
        }

        public IWorkItemLinkTypeEnd OppositeEnd
        {
            get { return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(new WorkItemLinkTypeEndProxy(_end.OppositeEnd)); }
        }
    }
}