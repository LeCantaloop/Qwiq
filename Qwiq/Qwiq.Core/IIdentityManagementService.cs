using System.Collections.Generic;

namespace Microsoft.IE.Qwiq
{
    public interface IIdentityManagementService
    {
        IEnumerable<ITeamFoundationIdentity> ReadIdentities(ICollection<IIdentityDescriptor> descriptors);

        IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(IdentitySearchFactor searchFactor, ICollection<string> searchFactorValues);

        IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier);
    }
}
