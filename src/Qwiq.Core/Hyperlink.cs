using System;
using System.Diagnostics;

namespace Microsoft.Qwiq
{
    public class Hyperlink : Link, IHyperlink
    {
        internal Hyperlink(string location, string comment = null)
            : base(comment, BaseLinkType.Hyperlink)
        {
            if (string.IsNullOrEmpty(location)) throw new ArgumentException("Value cannot be null or empty.", nameof(location));
            Location = location;
        }

        /// <inheritdoc />
        public string Location { get; }

        /// <inheritdoc />
        public bool Equals(IHyperlink other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;

            return StringComparer.OrdinalIgnoreCase.Equals(Location, other.Location);
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return Equals(obj as IHyperlink);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Location);
        }
    }
}