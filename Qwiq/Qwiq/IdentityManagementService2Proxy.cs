using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.IE.Qwiq
{
    public class IdentityManagementService2Proxy : IIdentityManagementService2
    {
        private readonly Tfs.IIdentityManagementService2 _identityManagementService2;

        public IdentityManagementService2Proxy(Tfs.IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2;
        }
    }
}