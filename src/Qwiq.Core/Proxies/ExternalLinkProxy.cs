using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    public class ExternalLinkProxy : LinkProxy, IExternalLink
    {
        private readonly Tfs.ExternalLink externalLink;

        internal ExternalLinkProxy(Tfs.ExternalLink externalLink) : base(externalLink)
        {
            this.externalLink = externalLink;
        }

        public string LinkedArtifactUri
        {
            get { return this.externalLink.LinkedArtifactUri; }
        }

        public string ArtifactLinkTypeName
        {
            get { return this.externalLink.ArtifactLinkType.Name; }
        }
    }
}
