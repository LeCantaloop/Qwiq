using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Identity
{
    public static class IdentityManagementServiceExtensions
    {
        private const string TfsLoggedInIdentityType = "Microsoft.IdentityModel.Claims.ClaimsIdentity";

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
                                .Where(identity => identity != null && !identity.IsContainer && identity.UniqueUserId == IdentityConstants.ActiveUniqueId)
                                .Select(i => i.GetUserAlias())
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToArray());
        }

        public static ITeamFoundationIdentity GetIdentityForAlias(this IIdentityManagementService ims, string alias,
            string tenantId, params string[] domains)
        {
            return GetIdentityForAliases(ims, new[] {alias}, tenantId, domains)[alias];
        }

        public static IDictionary<string, ITeamFoundationIdentity> GetIdentityForAliases(
            this IIdentityManagementService ims, ICollection<string> aliases, string tenantId, params string[] domains)
        {
            if (ims == null) throw new ArgumentNullException(nameof(ims));
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentNullException(nameof(tenantId));
            if (domains == null) throw new ArgumentNullException(nameof(domains));
            if (aliases == null) throw new ArgumentNullException(nameof(aliases));
            if (!domains.Any()) throw new ArgumentException(nameof(domains));
            if (!aliases.Any()) throw new ArgumentException(nameof(aliases));

            var descriptorsToAliasLookup = CreatePossibleIdentityDescriptors(ims, aliases, domains, tenantId);
            var identities = GetIdentitiesForAliases(ims, descriptorsToAliasLookup);

            var aliasesWithMissingIdentities = aliases.Except(identities.Keys);
            var searchedIdentities = SearchForIdentitiesForAliases(ims, aliasesWithMissingIdentities);

            return identities.Union(searchedIdentities).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static IEnumerable<KeyValuePair<string, ITeamFoundationIdentity>> SearchForIdentitiesForAliases(IIdentityManagementService ims, IEnumerable<string> aliases)
        {
            var missingIdentitiesList = ims.ReadIdentities(IdentitySearchFactor.AccountName, aliases.ToList());
            return missingIdentitiesList.Select(mil => new KeyValuePair<string, ITeamFoundationIdentity>(mil.Key, mil.Value.FirstOrDefault()));
        }

        private static IDictionary<string, ITeamFoundationIdentity> GetIdentitiesForAliases(IIdentityManagementService ims, IDictionary<string, ICollection<IIdentityDescriptor>> aliasDescriptors)
        {
            var descriptors = aliasDescriptors.SelectMany(ad => ad.Value).ToList();
            var descriptorToAliasLookup =
                aliasDescriptors
                    .SelectMany(
                        ad =>
                            ad.Value.Select(d => new KeyValuePair<string, string>(BuildDescriptorLookupKey(d), ad.Key)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
            var validIdentities = new Dictionary<string, ITeamFoundationIdentity>(StringComparer.OrdinalIgnoreCase);
            var lookupResults = ims.ReadIdentities(descriptors);
            foreach (var identity in lookupResults.Where(id => id != null))
            {
                var lookupKey = BuildDescriptorLookupKey(identity.Descriptor);
                var alias = descriptorToAliasLookup[lookupKey];
                if (!validIdentities.ContainsKey(alias))
                {
                    validIdentities.Add(alias, identity);
                }
            }

            return validIdentities;
        }

        private static IDictionary<string, ICollection<IIdentityDescriptor>> CreatePossibleIdentityDescriptors(IIdentityManagementService ims, ICollection<string> aliases, ICollection<string> domains, string tenantId)
        {
            var descriptors = new Dictionary<string, ICollection<IIdentityDescriptor>>();
            foreach (var alias in aliases)
            {
                var descriptorsForAlias = new List<IIdentityDescriptor>();
                foreach (var domain in domains)
                {
                    var loggedInAccountString = $"{tenantId}\\{alias}@{domain}";

                    descriptorsForAlias.Add(ims.CreateIdentityDescriptor(IdentityConstants.ClaimsType, loggedInAccountString));
                    descriptorsForAlias.Add(ims.CreateIdentityDescriptor(IdentityConstants.BindPendingIdentityType,
                        IdentityConstants.BindPendingSidPrefix + loggedInAccountString));
                }

                descriptors.Add(alias, descriptorsForAlias);
            }

            return descriptors;
        }
        private static string BuildDescriptorLookupKey(IIdentityDescriptor descriptor)
        {
            return $"{descriptor.IdentityType}-{descriptor.Identifier}}}";
        }
    }
}

