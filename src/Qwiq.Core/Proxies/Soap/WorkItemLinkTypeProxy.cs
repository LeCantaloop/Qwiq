using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class WorkItemLinkTypeProxy : IWorkItemLinkType
    {
        private readonly Tfs.WorkItemLinkType _linkType;

        private readonly Lazy<WorkItemLinkTypeEndProxy> _reverseEnd;
        private readonly Lazy<WorkItemLinkTypeEndProxy> _forwardEnd;

        internal WorkItemLinkTypeProxy(Tfs.WorkItemLinkType linkType)
        {
            _linkType = linkType;
            _reverseEnd = new Lazy<WorkItemLinkTypeEndProxy>(() => new WorkItemLinkTypeEndProxy(_linkType.ReverseEnd));
            _forwardEnd = new Lazy<WorkItemLinkTypeEndProxy>(() => new WorkItemLinkTypeEndProxy(_linkType.ForwardEnd));
        }

        public IWorkItemLinkTypeEnd ForwardEnd => _forwardEnd.Value;

        public bool IsActive => _linkType.IsActive;

        public string ReferenceName => _linkType.ReferenceName;

        public bool IsDirectional => _linkType.IsDirectional;

        public IWorkItemLinkTypeEnd ReverseEnd => _reverseEnd.Value;

        public override bool Equals(object obj)
        {
            if (!(obj is IWorkItemLinkType)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return WorkItemLinkTypeEqualityComparer.Instance.Equals(this, (IWorkItemLinkType)obj);
        }

        public override int GetHashCode()
        {
            return WorkItemLinkTypeEqualityComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return ReferenceName;
        }
    }
}
