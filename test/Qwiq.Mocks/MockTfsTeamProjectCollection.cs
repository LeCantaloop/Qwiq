using System;

using Microsoft.Qwiq.Identity;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Mocks
{
    public class MockTfsTeamProjectCollection : ITeamProjectCollection
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

        public VssCredentials AuthorizedCredentials { get; set; }

        public ITeamFoundationIdentity AuthorizedIdentity { get; set; }

        public ICommonStructureService CommonStructureService { get; set; }

        public bool HasAuthenticated { get; set; }

        public IIdentityManagementService IdentityManagementService { get; set; }

        public Uri Uri { get; set; }

        public TimeZone TimeZone { get; }
    }
}