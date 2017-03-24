using System;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemLinkTypeProxy : IWorkItemLinkType
    {
        public WorkItemLinkTypeProxy(string referenceName)
        {
            if (string.IsNullOrEmpty(referenceName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(referenceName));
            ReferenceName = referenceName;
        }

        public IWorkItemLinkTypeEnd ForwardEnd { get; internal set; }

        public bool IsActive { get; internal set; }

        public string ReferenceName { get; }

        public IWorkItemLinkTypeEnd ReverseEnd { get; internal set; }

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