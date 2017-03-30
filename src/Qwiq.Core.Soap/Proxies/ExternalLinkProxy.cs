using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class ExternalLinkProxy : LinkProxy, IExternalLink
    {
        internal ExternalLinkProxy(Tfs.ExternalLink externalLink) : base(externalLink)
        {
            LinkedArtifactUri = externalLink.LinkedArtifactUri;
            ArtifactLinkTypeName = externalLink.ArtifactLinkType.Name;
        }

        public string LinkedArtifactUri { get; }

        public string ArtifactLinkTypeName { get; }
    }
}
