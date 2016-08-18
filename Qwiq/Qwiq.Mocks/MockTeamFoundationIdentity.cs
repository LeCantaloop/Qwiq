using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockTeamFoundationIdentity : ITeamFoundationIdentity, IEquatable<ITeamFoundationIdentity>
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
            get
            {
                // Based on http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                var hash = 17;

                if (!string.IsNullOrEmpty(DisplayName)) hash = hash * 23 + DisplayName.GetHashCode();
                if (!string.IsNullOrEmpty(UniqueName)) hash = hash * 23 + UniqueName.GetHashCode();
                if (TeamFoundationId != null) hash = hash * 23 + TeamFoundationId.GetHashCode();
                if (Descriptor != null) hash = hash * 23 + Descriptor.GetHashCode();
                hash = hash * 23 + IsContainer.GetHashCode();

                return hash;
            }
        }

        public bool Equals(ITeamFoundationIdentity other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return UniqueUserId == other?.UniqueUserId;
        }
    }
}