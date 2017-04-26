using System.Diagnostics;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class Link : Qwiq.Link
    {
        [DebuggerStepThrough]
        internal Link(Tfs.Link link)
            : base(link.Comment, (BaseLinkType)link.BaseType)
        {
        }
    }
}