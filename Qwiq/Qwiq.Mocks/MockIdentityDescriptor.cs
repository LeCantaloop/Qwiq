namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockIdentityDescriptor : IIdentityDescriptor
    {
        /// <summary>
        /// Creates a ClaimsIdentity for the given <paramref name="alias"/>.
        /// </summary>
        /// <param name="alias">The sAMAccountName of the user (e.g. ftotten)</param>
        public MockIdentityDescriptor(string alias)
            : this("Microsoft.IdentityModel.Claims.ClaimsIdentity", $"CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C\\{alias}@domain.local")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identityType"></param>
        /// <param name="identifier"></param>
        /// <example>
        /// User:
        /// "Microsoft.IdentityModel.Claims.ClaimsIdentity", "2fa3a376-370f-4226-9fbb-d778e4b5bf74\\ftotten@fabrikam.com"
        /// 
        /// Service:
        /// "Microsoft.TeamFoundation.ServiceIdentity", "d9454f90-6587-4699-9357-3e83e331580a:Build:f2200ea9-52cf-4343-8c80-af2cfa409984"
        /// 
        /// TFS Identity:
        /// "Microsoft.TeamFoundation.Identity", "S-1-9-1234567890-1234567890-123456789-1234567890-1234567890-1-1234567890-1234567890-1234567890-1234567890"
        /// </example>
        public MockIdentityDescriptor(string identityType, string identifier)
        {
            IdentityType = identityType;
            Identifier = identifier;
        }

        public string Identifier { get; }

        public string IdentityType { get; }
    }
}