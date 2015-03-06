using System.Collections.Generic;
using System.Linq;
using Tfs = Microsoft.TeamFoundation.Framework;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class IdentityManagementServiceProxy : IIdentityManagementService
    {
        private readonly Tfs.Client.IIdentityManagementService2 _identityManagementService2;

        internal IdentityManagementServiceProxy(Tfs.Client.IIdentityManagementService2 identityManagementService2)
        {
            _identityManagementService2 = identityManagementService2;
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IdentitySearchFactor searchFactor, string[] searchFactorValues)
        {
            var factor = (Tfs.Common.IdentitySearchFactor) searchFactor;
            var identities = _identityManagementService2.ReadIdentities(factor, searchFactorValues,
                Tfs.Common.MembershipQuery.None, Tfs.Common.ReadIdentityOptions.None)[0];

            return identities.Select(item => new TeamFoundationIdentityProxy(item));
        }
    }
}