using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class ExternalLink : Link, IExternalLink
    {
        internal ExternalLink(Tfs.ExternalLink externalLink)
            : base(externalLink)
        {
            LinkedArtifactUri = externalLink.LinkedArtifactUri;
            ArtifactLinkTypeName = externalLink.ArtifactLinkType.Name;
        }

        public string ArtifactLinkTypeName { get; }

        public string LinkedArtifactUri { get; }
    }
}