using System;

namespace Qwiq
{
    internal class ExternalLink : Link, IExternalLink
    {
        public ExternalLink(string uri, string name, string comment = null)
            : base(comment, BaseLinkType.ExternalLink)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Trim().Length < 1) throw new ArgumentNullException(nameof(name));

            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (uri.Trim().Length < 1) throw new ArgumentNullException(nameof(uri));

            if (uri.Length > 2083) throw new ArgumentException("Uri too long.");

            if (Comparer.OrdinalIgnoreCase.Equals("Related Workitem", name)
                || Comparer.OrdinalIgnoreCase.Equals("Workitem Hyperlink", name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (Comparer.OrdinalIgnoreCase.Equals("Fixed in Changeset", name)
                || Comparer.OrdinalIgnoreCase.Equals("Source Code File", name)
                || Comparer.OrdinalIgnoreCase.Equals("Test Result", name))
            {
                throw new ArgumentException(nameof(uri));
            }

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