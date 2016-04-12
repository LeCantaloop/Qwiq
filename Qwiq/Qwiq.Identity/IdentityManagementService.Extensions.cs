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
            return GetAliasesForDisplayNames(ims, new[] {displayName})[displayName];
        }

        public static IDictionary<string, string[]> GetAliasesForDisplayNames(this IIdentityManagementService ims, string[] displayNames)
        {
            if (ims == null) throw new ArgumentNullException(nameof(ims));
            if (displayNames == null) throw new ArgumentNullException(nameof(displayNames));

            return ims.ReadIdentities(IdentitySearchFactor.DisplayName, displayNames)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp =>
                            kvp.Value
                                .Where(identity => identity != null && !identity.IsContainer && identity.IsActive)
                                .Select(i => i.GetUserAlias())
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToArray());
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
