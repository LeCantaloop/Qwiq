using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Qwiq.Identity;

namespace Microsoft.Qwiq.Mocks
{
    public class MockIdentityManagementService : IIdentityManagementService
    {
        [Obsolete("This field is depreciated and will be removed in a future version. Use Identities.Adamb instead.")]
        public static readonly ITeamFoundationIdentity Adamb = Identities.Adamb;

        [Obsolete("This field is depreciated and will be removed in a future version. Use Identities.Chrisj instead.")]
        public static readonly ITeamFoundationIdentity Chrisj = Identities.Chrisj;

        [Obsolete("This field is depreciated and will be removed in a future version. Use Identities.Chrisjoh instead.")]
        public static readonly ITeamFoundationIdentity Chrisjoh = Identities.Chrisjoh;

        [Obsolete("This field is depreciated and will be removed in a future version. Use Identities.Chrisjohn instead.")]
        public static readonly ITeamFoundationIdentity Chrisjohn = Identities.Chrisjohn;

        [Obsolete("This field is depreciated and will be removed in a future version. Use Identities.Chrisjohns instead.")]
        public static readonly ITeamFoundationIdentity Chrisjohns = Identities.Chrisjohns;

        [Obsolete("This field is depreciated and will be removed in a future version. Use Identities.Danj instead.")]
        public static readonly ITeamFoundationIdentity Danj = Identities.Danj;

        private readonly IDictionary<string, ITeamFoundationIdentity[]> _accountNameMappings;

        private readonly IDictionary<IIdentityDescriptor, ITeamFoundationIdentity> _descriptorMappings;

        /// <summary>
        ///     Initializes a new instance of the IMS with Contoso users (danj, adamb, chrisj, chrisjoh, chrisjohn, chrisjohns)
        /// </summary>
        public MockIdentityManagementService()
            : this(Identities.All)
        {
        }

        public MockIdentityManagementService(params ITeamFoundationIdentity[] identities)
            : this(identities as IEnumerable<ITeamFoundationIdentity>)
        {
        }

        public MockIdentityManagementService(IEnumerable<ITeamFoundationIdentity> identities)
            : this(identities.ToDictionary(k => new IdentityFieldValue(k).LogonName, e => e, StringComparer.OrdinalIgnoreCase))
        {
        }

        /// <summary>
        ///     Creates a new instance of the IMS
        /// </summary>
        /// <param name="accountNameMappings">
        ///     Collection of alias to <see cref="ITeamFoundationIdentity" />.
        /// </param>
        public MockIdentityManagementService(IDictionary<string, ITeamFoundationIdentity> accountNameMappings)
        {
            if (accountNameMappings == null) throw new ArgumentNullException(nameof(accountNameMappings));

            _accountNameMappings = new Dictionary<string, ITeamFoundationIdentity[]>(StringComparer.OrdinalIgnoreCase);
            _descriptorMappings = new Dictionary<IIdentityDescriptor, ITeamFoundationIdentity>(IdentityDescriptorComparer.Default);

            foreach (var account in accountNameMappings)
            {
                var success = _accountNameMappings.TryAdd(account.Key, new[] { account.Value });
                if (!success) Trace.TraceWarning("Account {0} not added; account already exists", account.Key);
            }

            foreach (var accounts in _accountNameMappings.Values)
            {
                foreach (var account in accounts)
                {
                    var success = _descriptorMappings.TryAdd(account.Descriptor, account);
                    if (!success) Trace.TraceWarning("Account {0} not added; account already exists", account.Descriptor.Identifier);
                }
            }
        }

        /// <summary>
        ///     Creates a new instance of the IMS
        /// </summary>
        /// <param name="userMappings">
        ///     Collection of alias and display names for which to initialize the IMS
        /// </param>
        public MockIdentityManagementService(IDictionary<string, string> userMappings)
            : this(
                   userMappings.ToDictionary(
                                             kvp => kvp.Key,
                                             kvp => (ITeamFoundationIdentity)new MockTeamFoundationIdentity(
                                                                                                            MockIdentityDescriptor
                                                                                                                    .Create(kvp.Key),
                                                                                                            kvp.Value,
                                                                                                            Guid.Empty)))
        {
        }

        public MockIdentityManagementService(IDictionary<string, IEnumerable<ITeamFoundationIdentity>> accountMappings)
        {
            _accountNameMappings = accountMappings?.ToDictionary(k => k.Key, e => e.Value.ToArray())
                                   ?? new Dictionary<string, ITeamFoundationIdentity[]>(StringComparer.OrdinalIgnoreCase);
            _descriptorMappings = new Dictionary<IIdentityDescriptor, ITeamFoundationIdentity>(IdentityDescriptorComparer.Default);

            foreach (var accounts in _accountNameMappings.Values)
            {
                foreach (var account in accounts) _descriptorMappings.Add(account.Descriptor, account);
            }
        }

        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return new IdentityDescriptor(identityType, identifier);
        }

        /// <summary>
        ///     Read identities for given descriptors.
        /// </summary>
        /// <param name="descriptors">
        ///     Collection of <see cref="IIdentityDescriptor" />
        /// </param>
        /// <returns>
        ///     An array of <see cref="ITeamFoundationIdentity" />, corresponding 1 to 1 with input descriptor array.
        /// </returns>
        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors)
        {
            return ReadIdentities(descriptors, MembershipQuery.None);
        }

        /// <summary>
        ///     Read identities for given descriptors.
        /// </summary>
        /// <param name="descriptors">
        ///     Collection of <see cref="IIdentityDescriptor" />
        /// </param>
        /// <param name="queryMembership"></param>
        /// <returns>
        ///     An array of <see cref="ITeamFoundationIdentity" />, corresponding 1 to 1 with input descriptor array.
        /// </returns>
        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(IEnumerable<IIdentityDescriptor> descriptors, MembershipQuery queryMembership)
        {
            foreach (var descriptor in descriptors)
            {
                var success = _descriptorMappings.TryGetValue(descriptor, out ITeamFoundationIdentity identity);

                Trace.TraceInformation($"{nameof(MockIdentityManagementService)}: Searching for {descriptor}; Success: {success}");

                yield return identity;
            }
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            IEnumerable<string> searchFactorValues)
        {
            return ReadIdentities(searchFactor, searchFactorValues, MembershipQuery.None);
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(
            IdentitySearchFactor searchFactor,
            IEnumerable<string> searchFactorValues,
            MembershipQuery queryMembership)
        {
            Trace.TraceInformation($"Searching for {searchFactor}: {string.Join(", ", searchFactorValues)}");

            switch (searchFactor)
            {
                // Alternate login username
                case IdentitySearchFactor.Alias:
                    foreach (var keyValuePair in SearchByAlias(searchFactorValues)) yield return keyValuePair;
                    break;
                // Windows NT account name: domain\alias or user@domain.tld
                case IdentitySearchFactor.AccountName:
                    foreach (var keyValuePair1 in SearchByAccountName(searchFactorValues)) yield return keyValuePair1;
                    break;
                // Display name
                case IdentitySearchFactor.DisplayName:
                    foreach (var keyValuePair2 in SearchByDisplayName(searchFactorValues)) yield return keyValuePair2;
                    break;

                case IdentitySearchFactor.AdministratorsGroup:
                case IdentitySearchFactor.Identifier:
                case IdentitySearchFactor.MailAddress:
                case IdentitySearchFactor.General:
                    throw new NotSupportedException();
            }
        }

        public ITeamFoundationIdentity ReadIdentity(
            IdentitySearchFactor searchFactor,
            string searchFactorValue)
        {
            return ReadIdentity(searchFactor, searchFactorValue, MembershipQuery.None);
        }

        /// <inheritdoc />
        public ITeamFoundationIdentity ReadIdentity(IdentitySearchFactor searchFactor, string searchFactorValue, MembershipQuery queryMembership)
        {
            return ReadIdentities(searchFactor, new[] { searchFactorValue }, queryMembership).First().Value.SingleOrDefault();
        }

        private IEnumerable<ITeamFoundationIdentity> Locate(Func<ITeamFoundationIdentity, bool> predicate)
        {
            return _accountNameMappings.Values.SelectMany(a => a, (a, i) => new { a, i }).Where(t => predicate(t.i)).Select(t => t.i)
                                       .ToArray();
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> SearchByAccountName(
            IEnumerable<string> searchFactors)
        {
            // TFS Matches, ignoring case, the property "Account", which is in the property bag of the identity

            return searchFactors.Select(
                                        searchFactor =>
                                            {
                                                bool Predicate(ITeamFoundationIdentity identity)
                                                {
                                                    return Comparer.OrdinalIgnoreCase.Equals(identity.GetUserAccountName(), searchFactor);
                                                }

                                                return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                                                                                                      searchFactor,
                                                                                                                      Locate(Predicate));
                                            });
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> SearchByAlias(IEnumerable<string> searchFactors)
        {
            // TFS Matches, ignoring case, the property "Alias", which is in the property bag of the identity
            // NOTE: This may not be populated for all identities

            foreach (var searchFactor in searchFactors)
            {
                if (_accountNameMappings.ContainsKey(searchFactor))
                    yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                                                                                searchFactor,
                                                                                                _accountNameMappings[searchFactor]);
                else
                    yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                                                                                searchFactor,
                                                                                                new ITeamFoundationIdentity[0]);
            }
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> SearchByDisplayName(
            IEnumerable<string> searchFactors)
        {
            // TFS Matches, ignoring case, the property "Display Name" on the identity

            return searchFactors.Select(
                                        searchFactor =>
                                            {
                                                bool Predicate(ITeamFoundationIdentity identity)
                                                {
                                                    return Comparer.OrdinalIgnoreCase.Equals(identity.DisplayName, searchFactor);
                                                }

                                                return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                                                                                                      searchFactor,
                                                                                                                      Locate(Predicate));
                                            });
        }
    }
}