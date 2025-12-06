using System;
using System.Diagnostics.CodeAnalysis;

using Qwiq.Identity;
using Microsoft.VisualStudio.Services.Common;

namespace Qwiq.Mocks
{
    /// <summary>
    /// Mock implementation of <see cref="ITeamProjectCollection"/> for testing.
    /// </summary>
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "MockTfsTeamProjectCollection mirrors the TFS/Azure DevOps API naming convention.")]
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