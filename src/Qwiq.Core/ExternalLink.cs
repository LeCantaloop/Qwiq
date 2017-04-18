using System;

namespace Microsoft.Qwiq
{
    internal class ExternalLink : Link, IExternalLink
    {
        public ExternalLink(string uri, string name, string comment = null)
            : base(comment, BaseLinkType.ExternalLink)
        {
            if (string.IsNullOrEmpty(uri)) throw new ArgumentException("Value cannot be null or empty.", nameof(uri));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));

            LinkedArtifactUri = uri;
            ArtifactLinkTypeName = name;
        }

        public string ArtifactLinkTypeName { get; }

        public string LinkedArtifactUri { get; }

        public bool Equals(IExternalLink other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;

            return StringComparer.OrdinalIgnoreCase.Equals(ArtifactLinkTypeName, other.ArtifactLinkTypeName)
                   && StringComparer.OrdinalIgnoreCase.Equals(LinkedArtifactUri, other.LinkedArtifactUri);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as IExternalLink);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(LinkedArtifactUri);
                hash = (13 * hash) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(ArtifactLinkTypeName);
                return hash;
            }
        }
    }
}