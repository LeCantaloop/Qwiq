using System;


namespace Qwiq
{
    public class WorkItemLinkTypeEnd : IWorkItemLinkTypeEnd, IEquatable<IWorkItemLinkTypeEnd>

    {

        private IWorkItemLinkTypeEnd _oppositeEnd = null!;
        private readonly Lazy<IWorkItemLinkTypeEnd> _lazyOpposite = null!;

        internal WorkItemLinkTypeEnd(string immutableName, IWorkItemLinkTypeEnd oppositeEnd)
            : this(immutableName)
        {
            _oppositeEnd = oppositeEnd ?? throw new ArgumentNullException(nameof(oppositeEnd));
        }

        internal WorkItemLinkTypeEnd(string immutableName, Lazy<IWorkItemLinkTypeEnd> oppositeEnd)
            : this(immutableName)
        {
            _lazyOpposite = oppositeEnd ?? throw new ArgumentNullException(nameof(oppositeEnd));
        }

        internal WorkItemLinkTypeEnd(string immutableName)
        {
            if (string.IsNullOrWhiteSpace(immutableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(immutableName));

            ImmutableName = string.Intern(immutableName);

            _lazyOpposite = new Lazy<IWorkItemLinkTypeEnd>(() => !IsForwardLink ? LinkType.ForwardEnd : LinkType.ReverseEnd);
        }



        public string ImmutableName { get; }

        public bool IsForwardLink { get; internal set; }

        public IWorkItemLinkType LinkType { get; internal set; } = null!;

        public string Name { get; internal set; } = null!;

        public IWorkItemLinkTypeEnd OppositeEnd => _oppositeEnd ?? (_oppositeEnd = _lazyOpposite.Value);

        public bool Equals(IWorkItemLinkTypeEnd? other)
        {
            return WorkItemLinkTypeEndComparer.Default.Equals(this, other);
        }

        public override bool Equals(object? obj)
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