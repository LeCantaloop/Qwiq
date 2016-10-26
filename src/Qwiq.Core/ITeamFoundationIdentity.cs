using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface ITeamFoundationIdentity
    {
        IIdentityDescriptor Descriptor { get; }
        string DisplayName { get; }
        bool IsActive { get; }
        bool IsContainer { get; }
        IEnumerable<IIdentityDescriptor> MemberOf { get; }
        IEnumerable<IIdentityDescriptor> Members { get; }
        Guid TeamFoundationId { get; }
        string UniqueName { get; }
        int UniqueUserId { get; }
    }
}

