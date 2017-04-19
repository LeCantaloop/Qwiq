using System;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkTypeEnd : IWorkItemLinkTypeEnd, IEquatable<IWorkItemLinkTypeEnd>

    {
        private readonly Lazy<IWorkItemLinkTypeEnd> _opposite;

        internal WorkItemLinkTypeEnd(string immutableName, Lazy<IWorkItemLinkTypeEnd> oppositeEnd)
            : this(immutableName)
        {
            _opposite = oppositeEnd;
        }

        internal WorkItemLinkTypeEnd(string immutableName)
        {
            ImmutableName = immutableName;
            if (string.IsNullOrWhiteSpace(immutableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(immutableName));
            _opposite = new Lazy<IWorkItemLinkTypeEnd>(() => !IsForwardLink ? LinkType.ForwardEnd : LinkType.ReverseEnd);
        }

        public int Id { get; internal set; }

        public string ImmutableName { get; }

        public bool IsForwardLink { get; internal set; }

        public IWorkItemLinkType LinkType { get; internal set; }

        public string Name { get; internal set; }

        public IWorkItemLinkTypeEnd OppositeEnd => _opposite.Value;

        public bool Equals(IWorkItemLinkTypeEnd other)
        {
            return WorkItemLinkTypeEndComparer.Default.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return WorkItemLinkTypeEndComparer.Default.Equals(this, obj as IWorkItemLinkTypeEnd);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkTypeEndComparer.Default.GetHashCode(this);
        }

        public override string ToString()
        {
            return ImmutableName;
        }
    }
}