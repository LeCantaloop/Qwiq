using System.Collections.Generic;

namespace Microsoft.IE.Qwiq
{
    public interface IIdentityManagementService
    {
        IEnumerable<ITeamFoundationIdentity> ReadIdentities(IdentitySearchFactor searchFactor,
            string[] searchFactorValues);
    }
}
