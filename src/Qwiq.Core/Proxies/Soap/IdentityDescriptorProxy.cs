using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class IdentityDescriptorProxy : IIdentityDescriptor
    {
        private readonly Tfs.IdentityDescriptor _descriptor;

        internal IdentityDescriptorProxy(Tfs.IdentityDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public string Identifier
        {
            get { return _descriptor.Identifier; }
        }

        public string IdentityType
        {
            get { return _descriptor.IdentityType; }
        }
    }
}
