using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class WorkItemLinkType : Qwiq.WorkItemLinkType
    {
        internal WorkItemLinkType(Tfs.WorkItemLinkType linkType)
            : base(
                linkType?.ReferenceName,
                new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEnd(linkType?.ForwardEnd)),
                new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEnd(linkType?.ReverseEnd)))
        {
            if (linkType == null) throw new ArgumentNullException(nameof(linkType));
            IsActive = linkType.IsActive;
            IsDirectional = linkType.IsDirectional;
        }
    }
}