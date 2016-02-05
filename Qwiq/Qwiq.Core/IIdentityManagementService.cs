using System.Collections.Generic;

namespace Microsoft.IE.Qwiq
{
    public interface IIdentityManagementService
    {
        IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors, MembershipQuery membershipQuery);
        IEnumerable<ITeamFoundationIdentity> ReadIdentities(IdentitySearchFactor searchFactor,
            string[] searchFactorValues, MembershipQuery membershipQuery);

        IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier);
    }
}
