using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Identity
{
    public static class IdentityManagementServiceExtensions
    {
        private const string TfsLoggedInIdentityType = "Microsoft.IdentityModel.Claims.ClaimsIdentity";
        private const string TfsBindPendingIdentityType = "Microsoft.TeamFoundation.BindPendingIdentity";
        public static string[] GetAliasesForDisplayName(this IIdentityManagementService ims, string displayName)
        {
            if (ims == null) throw new ArgumentNullException("ims");
            if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentNullException("displayName");

            var identities = ims.ReadIdentities(IdentitySearchFactor.DisplayName, new[] { displayName });

            var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var identity in identities.First().Value.Where(identity => identity != null && !identity.IsContainer && identity.IsActive))
            {
                aliases.Add(identity.GetUserAlias());
            }

            return aliases.ToArray();
        }

        public static ITeamFoundationIdentity GetIdentityForAlias(this IIdentityManagementService ims, string alias,
            string tenantId, params string[] domains)
        {
            if (ims == null) throw new ArgumentNullException("ims");
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentNullException("alias");
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentNullException("tenantId");
            if (domains == null) throw new ArgumentNullException("domains");
            if (!domains.Any()) throw new ArgumentException("domains");

            var numEndings = domains.Length;
            var possibleDescriptors = new List<IIdentityDescriptor>(numEndings * 2);
            for (var i = 0; i < numEndings; ++i)
            {
                var loggedInAccountString = string.Format("{0}\\{1}@{2}", tenantId, alias, domains[i]);

                possibleDescriptors.Add(ims.CreateIdentityDescriptor(TfsLoggedInIdentityType, loggedInAccountString));
                possibleDescriptors.Add(ims.CreateIdentityDescriptor(TfsBindPendingIdentityType,
                    "upn:" + loggedInAccountString));
            }
            var identities = ims.ReadIdentities(possibleDescriptors);
            var identity = identities.FirstOrDefault(id => id != null);

            if (identity == null)
            {
                // Perhaps this is an on-prem instance.  If that's the case, then this should work:
                var identitiesList = ims.ReadIdentities(IdentitySearchFactor.AccountName, new[] { alias }).ToArray();
                identity = identitiesList[0].Value.FirstOrDefault();
            }

            return identity;
        }
    }
}
