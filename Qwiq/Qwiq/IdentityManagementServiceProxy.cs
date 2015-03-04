using Tfs = Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.IE.Qwiq
{
    public class IdentityManagementServiceProxy : IIdentityManagementService
    {
        private readonly Tfs.IIdentityManagementService2 _identityManagementService2;

        internal IdentityManagementServiceProxy(Tfs.IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2;
        }
    }
}