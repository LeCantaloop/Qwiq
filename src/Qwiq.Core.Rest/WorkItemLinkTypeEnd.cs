using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    internal class WorkItemLinkTypeEnd : Qwiq.WorkItemLinkTypeEnd
    {
        internal WorkItemLinkTypeEnd(WorkItemRelationType item)
            : base(item?.ReferenceName)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Name = item.Name;
        }
    }
}