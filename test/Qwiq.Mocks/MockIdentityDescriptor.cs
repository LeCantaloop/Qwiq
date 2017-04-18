using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Mocks
{
    public class MockIdentityDescriptor : IdentityDescriptor
    {
        public const string Domain = "domain.local";

        public const string TenantId = "CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C";

        /// <summary>
        ///     Creates a ClaimsIdentity for the given <paramref name="alias" />.
        /// </summary>
        /// <param name="alias">The sAMAccountName of the user (e.g. ftotten)</param>
        public MockIdentityDescriptor(string alias, string domain = Domain, string tenantId = TenantId)
            : base(IdentityConstants.ClaimsType, $"{tenantId}\\{alias}@{domain}")
        {
        }
    }
}