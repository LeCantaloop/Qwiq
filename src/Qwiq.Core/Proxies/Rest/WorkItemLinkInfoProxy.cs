using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly WorkItemLink _item;

        internal WorkItemLinkInfoProxy(WorkItemLink item)
        {
            _item = item;
        }

        public bool IsLocked => throw new NotImplementedException();

        public int LinkTypeId => throw new NotImplementedException();

        public int SourceId => (_item.Source?.Id).GetValueOrDefault();

        public int TargetId => (_item.Target?.Id).GetValueOrDefault();
    }
}
