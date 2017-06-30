using System;
using System.Diagnostics.Contracts;
using System.Globalization;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkType : IWorkItemLinkType, IEquatable<IWorkItemLinkType>
    {
        private readonly Lazy<IWorkItemLinkTypeEnd> _forwardFac;

        private readonly Lazy<IWorkItemLinkTypeEnd> _reverseFac;

        private IWorkItemLinkTypeEnd _forward;

        private IWorkItemLinkTypeEnd _reverse;

        internal WorkItemLinkType([NotNull] string referenceName, [NotNull] IWorkItemLinkTypeEnd forward, [NotNull] IWorkItemLinkTypeEnd reverse)
            : this(referenceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(referenceName));
            Contract.Requires(forward != null);
            Contract.Requires(reverse != null);

            _forward = forward ?? throw new ArgumentNullException(nameof(forward));
            _reverse = reverse ?? throw new ArgumentNullException(nameof(reverse));
            _forwardFac = null;
            _reverseFac = null;
        }

        internal WorkItemLinkType([NotNull] string referenceName, [NotNull] Lazy<IWorkItemLinkTypeEnd> forward, [NotNull] Lazy<IWorkItemLinkTypeEnd> reverse)
            : this(referenceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(referenceName));
            Contract.Requires(forward != null);
            Contract.Requires(reverse != null);

            _forwardFac = forward ?? throw new ArgumentNullException(nameof(forward));
            _reverseFac = reverse ?? throw new ArgumentNullException(nameof(reverse));
        }

        internal WorkItemLinkType([NotNull] string referenceName)
        {
            Contract.Requires(!string.IsNullOrEmpty(referenceName));
            
            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));

            ReferenceName = string.Intern(referenceName);
        }

        public IWorkItemLinkTypeEnd ForwardEnd => CoerceForwardValue();

        public bool IsActive { get; internal set; }

        public bool IsDirectional { get; internal set; }

        public string ReferenceName { get; }

        public IWorkItemLinkTypeEnd ReverseEnd => CoerceReverseValue();

        public override bool Equals(object obj)
        {
            return WorkItemLinkTypeComparer.Default.Equals(this, obj as IWorkItemLinkType);
        }

        public bool Equals(IWorkItemLinkType other)
        {
            return WorkItemLinkTypeComparer.Default.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkTypeComparer.Default.GetHashCode(this);
        }

        public override string ToString()
        {
            return ReferenceName;
        }

        internal void SetReverseEnd(IWorkItemLinkTypeEnd value)
        {
            if (_reverse != null) throw new InvalidOperationException($"{nameof(ReverseEnd)} already contains a value.".ToString(CultureInfo.InvariantCulture));
            _reverse = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal void SetForwardEnd(IWorkItemLinkTypeEnd value)
        {
            if (_forward != null) throw new InvalidOperationException($"{nameof(ForwardEnd)} already contains a value.".ToString(CultureInfo.InvariantCulture));
            _forward = value ?? throw new ArgumentNullException(nameof(value));
        }

        private IWorkItemLinkTypeEnd CoerceForwardValue()
        {
            return _forward ?? (_forward = _forwardFac.Value);
        }

        private IWorkItemLinkTypeEnd CoerceReverseValue()
        {
            return _reverse ?? (_reverse = _reverseFac.Value);
        }
    }
}