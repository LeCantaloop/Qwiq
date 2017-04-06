using System;

namespace Microsoft.Qwiq.Rest
{
    public class IdentityDescriptor : Qwiq.IdentityDescriptor
    {
        internal IdentityDescriptor(VisualStudio.Services.Identity.IdentityDescriptor descriptor)
            : base(descriptor?.IdentityType, descriptor?.Identifier)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));

        }
    }
}