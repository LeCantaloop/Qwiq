using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class WorkItemLinkTypeProxy
    {
        internal WorkItemLinkTypeProxy(Tfs.WorkItemLinkType linkType)
            : this(
                new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEndProxy(linkType.ForwardEnd)),
                new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEndProxy(linkType.ReverseEnd)))
        {
            IsActive = linkType.IsActive;
            ReferenceName = linkType.ReferenceName;
            IsDirectional = linkType.IsDirectional;
        }
    }
}