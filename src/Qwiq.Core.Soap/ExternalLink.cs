using System.Diagnostics;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Qwiq.Client.Soap
{
    internal class ExternalLink : Qwiq.ExternalLink
    {
        [DebuggerStepThrough]
        internal ExternalLink(Tfs.ExternalLink externalLink)
            : base(externalLink.LinkedArtifactUri, externalLink.ArtifactLinkType.Name, externalLink.Comment)
        {
        }
    }
}