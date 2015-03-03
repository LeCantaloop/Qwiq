namespace Microsoft.IE.Qwiq
{
    public class IdentityManagementService2Proxy : IIdentityManagementService2
    {
        private readonly TeamFoundation.Framework.Client.IIdentityManagementService2 _identityManagementService2;

        public IdentityManagementService2Proxy(TeamFoundation.Framework.Client.IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2;
        }
    }
}