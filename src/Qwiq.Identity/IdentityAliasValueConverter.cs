using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Identity
{

    /// <summary>
    /// Converts a <see cref="string"/> representing an alias to a user principal name.
    /// </summary>
    /// <seealso cref="IIdentityValueConverter" />
    /// <seealso cref="IIdentityManagementService"/>
    public class IdentityAliasValueConverter : IIdentityValueConverter
    {
        private readonly string[] _domains;

        private readonly IIdentityManagementService _identityManagementService;

        private readonly string _tenantId;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityAliasValueConverter"/> class.
        /// </summary>
        /// <param name="identityManagementService">An instance of <see cref="IIdentityManagementService"/> to perform lookups.</param>
        /// <param name="tenantId">In a hosted VSTS instance, the tenantId to scope identity searches.</param>
        /// <param name="domains">A set of domains used to create <see cref="IIdentityDescriptor"/>s for search.</param>
        /// <example>
        /// var ims = ...;
        /// var mapper = new IdentityAliasMapper(ims, "CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C", "contoso.com");
        /// </example>
        public IdentityAliasValueConverter(
            [NotNull] IIdentityManagementService identityManagementService,
            [NotNull] string tenantId,
            [NotNull] [ItemNotNull] params string[] domains)
        {
            Contract.Requires(!string.IsNullOrEmpty(tenantId));
            Contract.Requires(identityManagementService != null);
            Contract.Requires(domains != null);
            Contract.Requires(domains.Length > 0);
            Contract.Requires(domains.All(item => item != null));

            if (domains == null) throw new ArgumentNullException(nameof(domains));
            if (string.IsNullOrEmpty(tenantId)) throw new ArgumentException("Value cannot be null or empty.", nameof(tenantId));
            if (domains.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(domains));
            _identityManagementService = identityManagementService ?? throw new ArgumentNullException(nameof(identityManagementService));
            _tenantId = tenantId;
            _domains = domains;
        }

        /// <summary>
        /// Converts the specified <paramref name="value" /> to an <see cref="object" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An <see cref="object" /> instance whose value is equivilent to the value of <paramref name="value" />.</returns>
        /// <example>"danj" becomes "danj@contoso.com"</example>
        [ContractAnnotation("null => null; notnull => notnull")]
        public object Map([CanBeNull] object value)
        {
            if (value is string stringValue) return GetIdentityNames(stringValue).Single();

            if (value is IEnumerable<string> stringArray) return GetIdentityNames(stringArray.ToArray());

            return value;
        }

        private string[] GetIdentityNames(params string[] aliases)
        {
            var identities = GetIdentityForAliases(aliases.ToList(), _tenantId, _domains);
            return identities.Select(i => i.Value.GetIdentityName() ?? i.Key).ToArray();
        }

        private IDictionary<string, ITeamFoundationIdentity> GetIdentityForAliases(
            ICollection<string> logonNames,
            string tenantId,
            params string[] domains)
        {
            if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentNullException(nameof(tenantId));
            if (domains == null) throw new ArgumentNullException(nameof(domains));
            if (logonNames == null) throw new ArgumentNullException(nameof(logonNames));
            if (!domains.Any()) throw new ArgumentException(nameof(domains));
            if (!logonNames.Any()) throw new ArgumentException(nameof(logonNames));

            var descriptorsToAliasLookup = CreatePossibleIdentityDescriptors(logonNames, domains, tenantId);
            var identities = GetIdentitiesForAliases(descriptorsToAliasLookup);
            var aliasesWithMissingIdentities = logonNames.Except(identities.Keys, Comparer.OrdinalIgnoreCase);
            foreach (var mil in aliasesWithMissingIdentities)
            {
                identities.Add(mil, null);
            }
            return identities;
        }

        private IDictionary<string, ICollection<IIdentityDescriptor>> CreatePossibleIdentityDescriptors(
            IEnumerable<string> aliases,
            string[] domains,
            string tenantId)
        {
            var descriptors = new Dictionary<string, ICollection<IIdentityDescriptor>>();
            foreach (var alias in aliases)
            {
                var descriptorsForAlias = new List<IIdentityDescriptor>();
                foreach (var domain in domains)
                {
                    var loggedInAccountString = $"{tenantId}\\{alias}@{domain}".ToString(CultureInfo.InvariantCulture);

                    descriptorsForAlias.Add(_identityManagementService.CreateIdentityDescriptor(IdentityConstants.ClaimsType, loggedInAccountString));
                    descriptorsForAlias.Add(_identityManagementService.CreateIdentityDescriptor(IdentityConstants.BindPendingIdentityType, IdentityConstants.BindPendingSidPrefix + loggedInAccountString));
                }

                descriptors.Add(alias, descriptorsForAlias);
            }

            return descriptors;
        }

        private IDictionary<string, ITeamFoundationIdentity> GetIdentitiesForAliases(
            IDictionary<string, ICollection<IIdentityDescriptor>> aliasDescriptors)
        {
            var descriptors = aliasDescriptors.SelectMany(ad => ad.Value).ToList();
            var descriptorToAliasLookup = aliasDescriptors
                    .SelectMany(ad => ad.Value.Select(d => new KeyValuePair<string, string>(d.ToString(), ad.Key)))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
            var validIdentities = new Dictionary<string, ITeamFoundationIdentity>(StringComparer.OrdinalIgnoreCase);
            var lookupResults = _identityManagementService.ReadIdentities(descriptors);
            foreach (var identity in lookupResults.Where(id => id != null))
            {
                var lookupKey = identity.Descriptor.ToString();
                var alias = descriptorToAliasLookup[lookupKey];
                if (!validIdentities.ContainsKey(alias)) validIdentities.Add(alias, identity);
            }

            return validIdentities;
        }
    }
}