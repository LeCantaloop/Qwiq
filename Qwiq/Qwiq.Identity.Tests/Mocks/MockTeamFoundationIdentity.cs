using System;
using System.Collections.Generic;
using Microsoft.IE.Qwiq;

namespace Qwiq.Identity.Tests.Mocks
{
    public class MockTeamFoundationIdentity : ITeamFoundationIdentity
    {
        public MockTeamFoundationIdentity(string alias, string displayName)
        {
            Descriptor = new MockIdentityDescriptor(alias);
            DisplayName = displayName;
            IsActive = true;
            IsContainer = false;
        }

        public IIdentityDescriptor Descriptor { get; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public bool IsContainer { get; }
        public IEnumerable<IIdentityDescriptor> MemberOf { get; }
        public IEnumerable<IIdentityDescriptor> Members { get; }
        public Guid TeamFoundationId { get; }
        public string UniqueName { get; }
        public int UniqueUserId { get; }
    }
}