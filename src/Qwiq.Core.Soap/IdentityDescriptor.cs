using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Soap
{
    public class IdentityDescriptor : IIdentityDescriptor
    {
        private readonly Tfs.IdentityDescriptor _descriptor;

        internal IdentityDescriptor(Tfs.IdentityDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public string Identifier => _descriptor.Identifier;

        public string IdentityType => _descriptor.IdentityType;
    }
}
