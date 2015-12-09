using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class WorkItemLinkTypeProxy : IWorkItemLinkType
    {
        private readonly Tfs.WorkItemLinkType _linkType;

        internal WorkItemLinkTypeProxy(Tfs.WorkItemLinkType linkType)
        {
            _linkType = linkType;
        }

        public IWorkItemLinkTypeEnd ForwardEnd
        {
            get { return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(new WorkItemLinkTypeEndProxy(_linkType.ForwardEnd)); }
        }

        public bool IsActive
        {
            get { return _linkType.IsActive; }
        }

        public string ReferenceName
        {
            get { return _linkType.ReferenceName; }
        }

        public IWorkItemLinkTypeEnd ReverseEnd
        {
            get { return ExceptionHandlingDynamicProxyFactory.Create<IWorkItemLinkTypeEnd>(new WorkItemLinkTypeEndProxy(_linkType.ReverseEnd)); }
        }
    }
}