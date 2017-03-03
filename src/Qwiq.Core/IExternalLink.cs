using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq
{
    public interface IExternalLink : ILink
    {
        string LinkedArtifactUri { get; }

        string ArtifactLinkTypeName { get; }
    }
}
