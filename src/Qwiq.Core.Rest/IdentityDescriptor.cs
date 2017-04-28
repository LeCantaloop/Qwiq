using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Client.Rest
{
    public class IdentityDescriptor : Qwiq.IdentityDescriptor
    {
        internal IdentityDescriptor([NotNull] VisualStudio.Services.Identity.IdentityDescriptor descriptor)
            : base(descriptor.IdentityType, descriptor.Identifier)
        {
            Contract.Requires(descriptor != null);
            
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
        }
    }
}