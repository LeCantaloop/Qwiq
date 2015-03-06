using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Identity
{
    public static class WorkItemStoreExtensions
    {
        private const string TfsInternalIdentityType = "Microsoft.TeamFoundation.Identity";
        private const string TfsLoggedInIdentityType = "Microsoft.IdentityModel.Claims.ClaimsIdentity";
        private const string TfsBindPendingIdentityType = "Microsoft.TeamFoundation.BindPendingIdentity";
        private const string MicrosoftTenantIdString = "72F988BF-86F1-41AF-91AB-2D7CD011DB47";
        private readonly static Guid MicrosoftTenantGuid = new Guid(MicrosoftTenantIdString);

        private readonly static string[] KnownUpnEndings =
        {
            "ntdev.microsoft.com",
            "microsoft.com",
            "ntdev.corp.microsoft.com",
            "windows.microsoft.com",
            "winse.microsoft.com",
            "corp.microsoft.com",
            "mds.microsoft.com"
        };

        public static string[] GetAliasesForDisplayName(this IIdentityManagementService ims, string displayName)
        {
            if (ims == null) throw new ArgumentNullException("ims");
            if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentNullException("displayName");

            var identities = ims.ReadIdentities(IdentitySearchFactor.DisplayName, new[] {displayName});

            var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var identity in identities)
            {
                if (identity != null && !identity.IsContainer && identity.IsActive)
                {
                    aliases.Add(identity.GetUserAlias());
                }
            }

            return aliases.ToArray();
        }

        public static ITeamFoundationIdentity GetIdentityForAlias(this IIdentityManagementService ims, string alias)
        {
            if (ims == null) throw new ArgumentNullException("ims");
            if (string.IsNullOrWhiteSpace(alias)) throw new ArgumentNullException("alias");

            var numEndings = KnownUpnEndings.Length;
            var possibleDescriptors = new List<IIdentityDescriptor>(numEndings * 2);
            for (var i = 0; i < numEndings; ++i)
            {
                var loggedInAccountString = string.Format("{0}\\{1}@{2}", MicrosoftTenantIdString, alias, KnownUpnEndings[i]);

                possibleDescriptors.Add(ims.CreateIdentityDescriptor(TfsLoggedInIdentityType, loggedInAccountString));
                possibleDescriptors.Add(ims.CreateIdentityDescriptor(TfsBindPendingIdentityType, "upn:" + loggedInAccountString));
            }
            var identities = ims.ReadIdentities(possibleDescriptors);
            var identity = identities.FirstOrDefault(id => id != null);

            if (identity == null)
            {
                // Perhaps this is an on-prem instance.  If that's the case, then this should work:
                var identitiesList = ims.ReadIdentities(IdentitySearchFactor.AccountName, new[] { alias }).ToArray();
                identity = identitiesList.Length == 0 ? null : identitiesList[0];
            }

            return identity;
        }

        public static string GetUserAlias(this ITeamFoundationIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException("identity");

            if (identity.Descriptor.Identifier.Contains("@"))
            {
                var identifier = identity.Descriptor.Identifier;
                var identifierSplit = identifier.Split('\\');

                if (identifierSplit.Length == 2)
                {
                    return identifierSplit[1].Split('@')[0];
                }
            }
            else
            {
                var uniqueName = identity.UniqueName;
                var uniqueNameSplit = uniqueName.Split('\\');
                if (uniqueNameSplit.Length == 2)
                {
                    return uniqueNameSplit[1];
                }
            }

            return null;
        }
    }
}
