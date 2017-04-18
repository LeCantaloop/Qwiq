using System;

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

        public ICommonStructureService CommonStructureService { get; set; }

        public IIdentityManagementService IdentityManagementService { get; set; }

        public TimeZone TimeZone { get; }

        public Uri Uri { get; set; }
    }
}