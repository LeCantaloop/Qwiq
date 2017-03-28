using System;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal partial class WorkItemLinkType : Qwiq.WorkItemLinkType
    {
        internal WorkItemLinkType(string referenceName)
            : base(referenceName)
        {
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