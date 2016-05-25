using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockTeamFoundationIdentity : ITeamFoundationIdentity
    {
        public MockTeamFoundationIdentity(string displayName, string uniqueName)
        {
            DisplayName = displayName;
            UniqueName = uniqueName;
            MemberOf = Enumerable.Empty<IIdentityDescriptor>();
            Members = Enumerable.Empty<IIdentityDescriptor>();
            IsActive = true;
            IsContainer = false;
            TeamFoundationId = Guid.Empty;
            Descriptor = new MockIdentityDescriptor(uniqueName);
        }

        public IIdentityDescriptor Descriptor { get; }

        public string DisplayName { get; }

        public bool IsActive { get; set; }

        public bool IsContainer { get; set; }

        public IEnumerable<IIdentityDescriptor> MemberOf { get; set; }

        public IEnumerable<IIdentityDescriptor> Members { get; set; }

        public Guid TeamFoundationId { get; set; }

        public string UniqueName { get; set; }

        public int UniqueUserId
        {
            get { throw new NotImplementedException(); }
        }
    }
}