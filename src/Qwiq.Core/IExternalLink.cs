using System;

namespace Qwiq
{
    public interface IExternalLink : ILink, IEquatable<IExternalLink>
    {
        string ArtifactLinkTypeName { get; }

        string LinkedArtifactUri { get; }
    }
}