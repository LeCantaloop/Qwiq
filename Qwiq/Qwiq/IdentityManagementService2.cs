namespace Microsoft.IE.Qwiq
{
    public class IdentityManagementService2 : IIdentityManagementService2
    {
        private readonly TeamFoundation.Framework.Client.IIdentityManagementService2 _identityManagementService2;

        public IdentityManagementService2(TeamFoundation.Framework.Client.IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2;
        }
    }
}