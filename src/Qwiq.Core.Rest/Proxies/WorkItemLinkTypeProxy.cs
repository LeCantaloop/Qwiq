using System;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal partial class WorkItemLinkTypeProxy : Microsoft.Qwiq.Proxies.WorkItemLinkTypeProxy
    {
        internal WorkItemLinkTypeProxy(
            string referenceName
            )
        {
            if (string.IsNullOrEmpty(referenceName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(referenceName));
            ReferenceName = referenceName;
        }

        internal void SetForwardEnd(IWorkItemLinkTypeEnd forward)
        {
            _forward = forward ?? throw new ArgumentNullException(nameof(forward));
        }

        internal void SetReverseEnd(IWorkItemLinkTypeEnd reverse)
        {
            _reverse = reverse ?? throw new ArgumentNullException(nameof(reverse));
        }
    }
}