using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class WorkItemLinkTypeProxy : Microsoft.Qwiq.Proxies.WorkItemLinkTypeProxy
    {
        internal WorkItemLinkTypeProxy(Tfs.WorkItemLinkType linkType)
            : base(
                new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEndProxy(linkType.ForwardEnd)),
                new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEndProxy(linkType.ReverseEnd)))
        {
            IsActive = linkType.IsActive;
            ReferenceName = linkType.ReferenceName;
            IsDirectional = linkType.IsDirectional;
        }
    }
}