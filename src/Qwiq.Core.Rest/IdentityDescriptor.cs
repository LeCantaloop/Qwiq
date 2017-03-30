using System;

namespace Microsoft.Qwiq.Rest
{
    internal class IdentityDescriptor : IIdentityDescriptor
    {
        internal IdentityDescriptor(VisualStudio.Services.Identity.IdentityDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            Identifier = descriptor.Identifier;
            IdentityType = descriptor.IdentityType;
        }

        public string Identifier { get; }

        public string IdentityType { get; }
    }
}