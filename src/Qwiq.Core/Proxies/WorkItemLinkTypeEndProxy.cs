using System;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemLinkTypeEndProxy : IWorkItemLinkTypeEnd
    {
        private readonly Lazy<IWorkItemLinkTypeEnd> _opposite;

        internal WorkItemLinkTypeEndProxy(Lazy<IWorkItemLinkTypeEnd> oppositeEnd)
            : this()
        {
            _opposite = oppositeEnd;
        }

        internal WorkItemLinkTypeEndProxy()
        {
            _opposite = new Lazy<IWorkItemLinkTypeEnd>(
                () => !IsForwardLink ? LinkType.ForwardEnd : LinkType.ReverseEnd);
        }

        public int Id { get; internal set; }

        public string ImmutableName { get; internal set; }

        public bool IsForwardLink { get; internal set; }

        public IWorkItemLinkType LinkType { get; internal set; }

        public string Name { get; internal set; }

        public IWorkItemLinkTypeEnd OppositeEnd => _opposite.Value;

        public override bool Equals(object obj)
        {
            return WorkItemLinkTypeEndComparer.Instance.Equals(this, obj as IWorkItemLinkTypeEnd);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkTypeEndComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return ImmutableName;
        }
    }
}