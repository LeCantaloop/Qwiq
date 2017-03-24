using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public struct WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly WorkItemLink _item;

        internal WorkItemLinkInfoProxy(WorkItemLink item)
        {
            _item = item;
        }

        public bool IsLocked => throw new NotImplementedException();

        public int LinkTypeId => throw new NotImplementedException();

        public int SourceId => (_item?.Source?.Id).GetValueOrDefault();

        public int TargetId => (_item?.Target?.Id).GetValueOrDefault();

        public static bool operator !=(WorkItemLinkInfoProxy x, WorkItemLinkInfoProxy y)
        {
            return !x.Equals(y);
        }

        public static bool operator ==(WorkItemLinkInfoProxy x, WorkItemLinkInfoProxy y)
        {
            return x.Equals(y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WorkItemLinkInfoProxy)) return false;
            return WorkItemLinkInfoEqualityComparer.Instance.Equals(this, (IWorkItemLinkInfo)obj);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkInfoEqualityComparer.Instance.GetHashCode(this);
        }
    }
}
