using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class WorkItemLinkTypeEndProxy : IWorkItemLinkTypeEnd
    {
        private readonly Tfs.WorkItemLinkTypeEnd _end;

        private readonly Lazy<IWorkItemLinkTypeEnd> _opposite;

        internal WorkItemLinkTypeEndProxy(Tfs.WorkItemLinkTypeEnd end)
        {
            _end = end;
            LinkType = new WorkItemLinkTypeProxy(_end.LinkType);
            _opposite = new Lazy<IWorkItemLinkTypeEnd>(() => new WorkItemLinkTypeEndProxy(_end.OppositeEnd));
        }

        public int Id => _end.Id;

        public string ImmutableName => _end.ImmutableName;

        public bool IsForwardLink => _end.IsForwardLink;

        public IWorkItemLinkType LinkType { get; }

        public string Name => _end.Name;

        public IWorkItemLinkTypeEnd OppositeEnd => _opposite.Value;

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