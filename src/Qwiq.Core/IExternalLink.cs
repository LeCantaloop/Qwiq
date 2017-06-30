using System;

namespace Microsoft.Qwiq
{
    public interface IExternalLink : ILink, IEquatable<IExternalLink>
    {
        string ArtifactLinkTypeName { get; }

        string LinkedArtifactUri { get; }
    }
}