using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class WorkItemLinkTypeEndProxy : Microsoft.Qwiq.Proxies.WorkItemLinkTypeEndProxy
    {
        internal WorkItemLinkTypeEndProxy(Tfs.WorkItemLinkTypeEnd end)
            : base(new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEndProxy(end.OppositeEnd)))
        {
            Id = end.Id;
            LinkType = new WorkItemLinkTypeProxy(end.LinkType);
            ImmutableName = end.ImmutableName;
            IsForwardLink = end.IsForwardLink;
            Name = end.Name;
        }
    }
}