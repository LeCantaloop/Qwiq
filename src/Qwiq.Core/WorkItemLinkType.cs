using System;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkType : IWorkItemLinkType, IComparable<IWorkItemLinkType>, IEquatable<IWorkItemLinkType>
    {
        private readonly Lazy<IWorkItemLinkTypeEnd> _forwardFac;

        private readonly Lazy<IWorkItemLinkTypeEnd> _reverseFac;

        protected IWorkItemLinkTypeEnd _forward;

        protected IWorkItemLinkTypeEnd _reverse;

        internal WorkItemLinkType(string referenceName, IWorkItemLinkTypeEnd forward, IWorkItemLinkTypeEnd reverse)
            : this(referenceName)
        {
            _forward = forward ?? throw new ArgumentNullException(nameof(forward));
            _reverse = reverse ?? throw new ArgumentNullException(nameof(reverse));
        }

        internal WorkItemLinkType(string referenceName, Lazy<IWorkItemLinkTypeEnd> forward, Lazy<IWorkItemLinkTypeEnd> reverse)
            : this(referenceName)
        {
            _forwardFac = forward ?? throw new ArgumentNullException(nameof(forward));
            _reverseFac = reverse ?? throw new ArgumentNullException(nameof(reverse));
        }

        internal WorkItemLinkType(string referenceName)
        {
            ReferenceName = referenceName;
            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));
        }

        public IWorkItemLinkTypeEnd ForwardEnd => _forward ?? _forwardFac.Value;

        public bool IsActive { get; internal set; }

        public bool IsDirectional { get; internal set; }

        public string ReferenceName { get; }

        public IWorkItemLinkTypeEnd ReverseEnd => _reverse ?? _reverseFac.Value;

        public override bool Equals(object obj)
        {
            return WorkItemLinkTypeComparer.Instance.Equals(this, obj as IWorkItemLinkType);
        }

        public int CompareTo(IWorkItemLinkType other)
        {
            return WorkItemLinkTypeComparer.Instance.Compare(this, other);
        }

        public bool Equals(IWorkItemLinkType other)
        {
            return WorkItemLinkTypeComparer.Instance.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkTypeComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return ReferenceName;
        }
    }
}