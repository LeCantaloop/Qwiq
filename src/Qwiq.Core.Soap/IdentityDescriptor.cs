using System;

using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class IdentityDescriptor : IIdentityDescriptor
    {
        private readonly Tfs.IdentityDescriptor _descriptor;

        internal IdentityDescriptor(Tfs.IdentityDescriptor descriptor)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
        }

        public string Identifier => _descriptor.Identifier;

        public string IdentityType => _descriptor.IdentityType;
    }
}
