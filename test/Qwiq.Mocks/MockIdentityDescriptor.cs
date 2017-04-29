using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Mocks
{
    public class MockIdentityDescriptor
    {
        public const string Domain = "domain.local";

        public const string TenantId = "CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C";



        public static IIdentityDescriptor Create(string alias, string domain = Domain, string tenantId = TenantId)
        {
            return new IdentityDescriptor(IdentityConstants.ClaimsType, $"{tenantId}\\{alias}@{domain}");
        }
    }


}