using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemLinkTypeEndProxy : IWorkItemLinkTypeEnd
    {
        private readonly WorkItemRelationType _item;

        public WorkItemLinkTypeEndProxy(WorkItemRelationType item)
        {
            _item = item;
        }

        public int Id { get; }

        public string ImmutableName => _item.ReferenceName;

        public bool IsForwardLink { get; internal set; }

        public IWorkItemLinkType LinkType { get; internal set; }

        public string Name => _item.Name;

        public IWorkItemLinkTypeEnd OppositeEnd { get; internal set; }

        public override bool Equals(object obj)
        {
            if (!(obj is IWorkItemLinkTypeEnd)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return WorkItemLinkTypeEndEqualityComparer.Instance.Equals(this, (IWorkItemLinkTypeEnd)obj);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkTypeEndEqualityComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return ImmutableName;
        }
    }
}