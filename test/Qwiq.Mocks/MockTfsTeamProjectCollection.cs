using System;

using Qwiq.Identity;
using Microsoft.VisualStudio.Services.Common;

namespace Qwiq.Mocks
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

        public VssCredentials AuthorizedCredentials { get; set; } = null!;

        public ITeamFoundationIdentity AuthorizedIdentity { get; set; } = null!;

        public ICommonStructureService? CommonStructureService { get; set; }

        public bool HasAuthenticated { get; set; }

        public IIdentityManagementService? IdentityManagementService { get; set; }

        public Uri Uri { get; set; } = null!;

        public TimeZone TimeZone { get; }
    }
}