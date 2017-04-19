using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public static class Identities
    {
        public static readonly ITeamFoundationIdentity Danj = new MockTeamFoundationIdentity(
                                                                                             new MockIdentityDescriptor("danj", "contoso.com"),
                                                                                             "Dan Jump",
                                                                                             Guid.Parse("b7de08a6-8417-491b-be62-85945a538f46"));

        public static readonly ITeamFoundationIdentity Adamb = new MockTeamFoundationIdentity(
                                                                                              new MockIdentityDescriptor("adamb", "contoso.com"),
                                                                                              "Adam Barr",
                                                                                              Guid.Parse("7846c22f-d3d8-4e02-8b62-d055d0284783"));

        public static readonly ITeamFoundationIdentity Chrisj = new MockTeamFoundationIdentity(
                                                                                               new MockIdentityDescriptor("chrisj", "contoso.com"),
                                                                                               "Chris Johnson",
                                                                                               Guid.Parse("f92c1baa-0038-4247-be68-12043fcc34e3"),
                                                                                               false);

        public static readonly ITeamFoundationIdentity Chrisjoh = new MockTeamFoundationIdentity(
                                                                                                 new MockIdentityDescriptor("chrisjoh", "contoso.com"),
                                                                                                 "Chris Johnson (FINANCE)",
                                                                                                 Guid.Parse("41e97533-89f7-45d7-8246-eaa449b5651d"));

        public static readonly ITeamFoundationIdentity Chrisjohn = new MockTeamFoundationIdentity(
                                                                                                  new MockIdentityDescriptor("chrisjohn", "contoso.com"),
                                                                                                  "Chris F. Johnson",
                                                                                                  Guid.Parse("b3da460c-6191-4725-b08d-52bba48a574f"));

        public static readonly ITeamFoundationIdentity Chrisjohns = new MockTeamFoundationIdentity(
                                                                                                   new MockIdentityDescriptor("chrisjohns", "contoso.com"),
                                                                                                   "Chris Johnson <chrisjohns@contoso.com>",
                                                                                                   Guid.Parse("67b42b6c-6bd8-40e2-a622-fe69eacd3d47"));

        public static readonly ITeamFoundationIdentity[] All = {
                                                                       Danj,
                                                                       Adamb,
                                                                       Chrisj,
                                                                       Chrisjoh,
                                                                       Chrisjohn,
                                                                       Chrisjohns
                                                                   };
    }

    public class MockIdentityManagementService : IIdentityManagementService
    {
        [Obsolete("This field is depreciated and will be removed in a future version. Use Identities.Danj instead.")]
        public static readonly ITeamFoundationIdentity Danj =Identities.Danj;

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


        private readonly IDictionary<string, ITeamFoundationIdentity[]> _accountNameMappings;
        private readonly IDictionary<IIdentityDescriptor, ITeamFoundationIdentity> _descriptorMappings;

        /// <summary>
        /// Initializes a new instance of the IMS with Contoso users (danj, adamb, chrisj, chrisjoh, chrisjohn, chrisjohns)
        /// </summary>
        public MockIdentityManagementService()
            : this(Identities.All)
        {
        }

        public MockIdentityManagementService(IEnumerable<ITeamFoundationIdentity> identities)
            : this(identities.ToDictionary(k => k.GetUserAlias(), e => e, StringComparer.OrdinalIgnoreCase))
        {
        }



        /// <summary>
        /// Creates a new instance of the IMS
        /// </summary>
        /// <param name="accountNameMappings">
        /// Collection of alias to <see cref="ITeamFoundationIdentity"/>.
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
        /// Creates a new instance of the IMS
        /// </summary>
        /// <param name="userMappings">
        /// Collection of alias and display names for which to initialize the IMS
        /// </param>
        public MockIdentityManagementService(IDictionary<string, string> userMappings)
            : this(userMappings.ToDictionary(kvp => kvp.Key, kvp => new MockTeamFoundationIdentity(kvp.Value, kvp.Key + "@domain.local") as ITeamFoundationIdentity))
        {
        }

        public MockIdentityManagementService(IDictionary<string, IEnumerable<ITeamFoundationIdentity>> accountMappings)
        {
            _accountNameMappings = accountMappings?.ToDictionary(k => k.Key, e => e.Value.ToArray())
                                    ?? new Dictionary<string, ITeamFoundationIdentity[]>(StringComparer.OrdinalIgnoreCase);
            _descriptorMappings = new Dictionary<IIdentityDescriptor, ITeamFoundationIdentity>(IdentityDescriptorComparer.Default);

            foreach (var accounts in _accountNameMappings.Values)
            {
                foreach (var account in accounts)
                {
                    _descriptorMappings.Add(account.Descriptor, account);
                }
            }
        }


        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return new MockIdentityDescriptor(identityType, identifier);
        }

        /// <summary>
        /// Read identities for given descriptors.
        /// </summary>
        /// <param name="descriptors">Collection of <see cref="IIdentityDescriptor"/></param>
        /// <returns>
        /// An array of <see cref="ITeamFoundationIdentity"/>, corresponding 1 to 1 with input descriptor array.
        /// </returns>
        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(ICollection<IIdentityDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                _descriptorMappings.TryGetValue(descriptor, out ITeamFoundationIdentity identity);
                yield return identity;
            }
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(IdentitySearchFactor searchFactor, ICollection<string> searchFactorValues)
        {
            switch (searchFactor)
            {
                // Alternate login username
                case IdentitySearchFactor.Alias:
                    foreach (var value in searchFactorValues)
                    {
                        if (_accountNameMappings.ContainsKey(value))
                        {
                            yield return
                                new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                    value,
                                    _accountNameMappings[value]);
                        }
                        else
                        {
                            yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(value, new ITeamFoundationIdentity[0]);
                        }
                    }
                    break;
                // Windows NT account name: domain\alias.
                case IdentitySearchFactor.AccountName:
                    foreach (var value in searchFactorValues)
                    {
                        yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                            value,
                            Locate(identity => identity.GetAttribute("Account", string.Empty).StartsWith(value, StringComparison.OrdinalIgnoreCase)));
                    }
                    break;
                // Display name
                case IdentitySearchFactor.DisplayName:
                    foreach (var value in searchFactorValues)
                    {
                        yield return
                            new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                                value,
                                Locate(identity =>
                                value.Equals(identity.DisplayName, StringComparison.OrdinalIgnoreCase) ||
                                identity.DisplayName.StartsWith(value, StringComparison.OrdinalIgnoreCase)));
                    }
                    break;
                case IdentitySearchFactor.AdministratorsGroup:
                case IdentitySearchFactor.Identifier:
                case IdentitySearchFactor.MailAddress:
                case IdentitySearchFactor.General:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<ITeamFoundationIdentity> Locate(Func<ITeamFoundationIdentity, bool> predicate)
        {
            return _accountNameMappings
                        .Values
                        .SelectMany(a => a, (a, i) => new { a, i })
                        .Where(t => predicate(t.i))
                        .Select(t => t.i)
                        .ToArray();
        }
    }
}

