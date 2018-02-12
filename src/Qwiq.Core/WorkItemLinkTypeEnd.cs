using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Qwiq
{
    public class WorkItemLinkTypeEnd : IWorkItemLinkTypeEnd, IEquatable<IWorkItemLinkTypeEnd>

    {

        private IWorkItemLinkTypeEnd _oppositeEnd;

        [CanBeNull]
        private readonly Lazy<IWorkItemLinkTypeEnd> _lazyOpposite;

        internal WorkItemLinkTypeEnd([NotNull] string immutableName, [NotNull] IWorkItemLinkTypeEnd oppositeEnd)
        {
            Contract.Requires(!string.IsNullOrEmpty(immutableName));
            Contract.Requires(oppositeEnd != null);
            _oppositeEnd = oppositeEnd ?? throw new ArgumentNullException(nameof(oppositeEnd));
        }

        internal WorkItemLinkTypeEnd([NotNull] string immutableName, [NotNull] Lazy<IWorkItemLinkTypeEnd> oppositeEnd)
            : this(immutableName)
        {
            Contract.Requires(!string.IsNullOrEmpty(immutableName));
            Contract.Requires(oppositeEnd != null);

            _lazyOpposite = oppositeEnd ?? throw new ArgumentNullException(nameof(oppositeEnd));
        }

        internal WorkItemLinkTypeEnd([NotNull] string immutableName)
        {
            Contract.Requires(!string.IsNullOrEmpty(immutableName));
            if (string.IsNullOrWhiteSpace(immutableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(immutableName));

            ImmutableName = string.Intern(immutableName);

            _lazyOpposite = new Lazy<IWorkItemLinkTypeEnd>(() => !IsForwardLink ? LinkType.ForwardEnd : LinkType.ReverseEnd);
        }



        public string ImmutableName { get; }

        public bool IsForwardLink { get; internal set; }

        public IWorkItemLinkType LinkType { get; internal set; }

        public string Name { get; internal set; }

        public IWorkItemLinkTypeEnd OppositeEnd => _oppositeEnd ?? (_oppositeEnd = _lazyOpposite.Value);

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