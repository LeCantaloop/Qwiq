using System;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class WorkItemLinkTypeProxy : IWorkItemLinkType
    {
        private readonly Lazy<IWorkItemLinkTypeEnd> _forwardFac;

        private readonly Lazy<IWorkItemLinkTypeEnd> _reverseFac;

        private IWorkItemLinkTypeEnd _forward;

        private IWorkItemLinkTypeEnd _reverse;

        internal WorkItemLinkTypeProxy(IWorkItemLinkTypeEnd forward, IWorkItemLinkTypeEnd reverse)
        {
            _forward = forward ?? throw new ArgumentNullException(nameof(forward));
            _reverse = reverse ?? throw new ArgumentNullException(nameof(reverse));
        }

        internal WorkItemLinkTypeProxy(Lazy<IWorkItemLinkTypeEnd> forward, Lazy<IWorkItemLinkTypeEnd> reverse)
        {
            _forwardFac = forward ?? throw new ArgumentNullException(nameof(forward));
            _reverseFac = reverse ?? throw new ArgumentNullException(nameof(reverse));
        }

        private WorkItemLinkTypeProxy()
        {
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