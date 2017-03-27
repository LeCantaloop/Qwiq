using System;

using Microsoft.Qwiq.Credentials;

namespace Microsoft.Qwiq.Mocks
{
    public class MockTfsTeamProjectCollection : ITfsTeamProjectCollection
    {
        public MockTfsTeamProjectCollection()
            : this(new MockIdentityManagementService())
        {
        }

        public MockTfsTeamProjectCollection(IIdentityManagementService identityManagementService)
        {
            IdentityManagementService = identityManagementService;
            TimeZone = TimeZone.CurrentTimeZone;
        }

        public TfsCredentials AuthorizedCredentials { get; set; }

        public ITeamFoundationIdentity AuthorizedIdentity { get; set; }

        public ICommonStructureService CommonStructureService { get; set; }

        public bool HasAuthenticated { get; set; }

        public IIdentityManagementService IdentityManagementService { get; set; }

        public Uri Uri { get; set; }

        public TimeZone TimeZone { get; }
    }
}