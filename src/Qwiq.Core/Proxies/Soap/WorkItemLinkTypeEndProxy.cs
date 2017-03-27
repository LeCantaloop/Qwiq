using System;

using Microsoft.Qwiq.Proxies.Soap;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class WorkItemLinkTypeEndProxy
    {
        internal WorkItemLinkTypeEndProxy(Tfs.WorkItemLinkTypeEnd end)
            : this(new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEndProxy(end.OppositeEnd)))
        {
            Id = end.Id;
            LinkType = new WorkItemLinkTypeProxy(end.LinkType);
            ImmutableName = end.ImmutableName;
            IsForwardLink = end.IsForwardLink;
            Name = end.Name;
        }
    }
}