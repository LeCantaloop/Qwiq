using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.IE.Qwiq.Mocks
{
    public class MockIdentityManagementService : IIdentityManagementService
    {
        private readonly IDictionary<string, IEnumerable<ITeamFoundationIdentity>> _accountNameMappings;

        /// <summary>
        /// Initializes a new instance of MockIdentityManagementService with Contoso users
        /// </summary>
        public MockIdentityManagementService()
            : this(new Dictionary<string, ITeamFoundationIdentity>(StringComparer.OrdinalIgnoreCase)
                        {
                            {"danj", new MockTeamFoundationIdentity("Dan Jump", "danj@contoso.com") {TeamFoundationId = Guid.Parse("b7de08a6-8417-491b-be62-85945a538f46")} },
                            {"adamb", new MockTeamFoundationIdentity("Adam Barr", "adamb@contoso.com") {TeamFoundationId = Guid.Parse("7846c22f-d3d8-4e02-8b62-d055d0284783")} },
                            {"chrisj", new MockTeamFoundationIdentity("Chris Johnson", "chrisj@contoso.com") {TeamFoundationId = Guid.Parse("f92c1baa-0038-4247-be68-12043fcc34e3"), IsActive = false} },
                            {"chrisjoh", new MockTeamFoundationIdentity("Chris Johnson (FINANCE)", "chrisjoh@contoso.com") {TeamFoundationId = Guid.Parse("41e97533-89f7-45d7-8246-eaa449b5651d")} },
                            {"chrisjohn", new MockTeamFoundationIdentity("Chris F. Johnson", "chrisjohn@contoso.com") {TeamFoundationId = Guid.Parse("b3da460c-6191-4725-b08d-52bba48a574f")} },
                            {"chrisjohns", new MockTeamFoundationIdentity("Chris Johnson <chrisjohns@contoso.com>", "chrisjohns@contoso.com") {TeamFoundationId = Guid.Parse("67b42b6c-6bd8-40e2-a622-fe69eacd3d47")} }
                        })
        {
        }

        public MockIdentityManagementService(IDictionary<string, ITeamFoundationIdentity> accountNameMappings)
        {
            _accountNameMappings = new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>(StringComparer.OrdinalIgnoreCase);
            foreach (var account in accountNameMappings)
            {
                _accountNameMappings.Add(account.Key, new[] { account.Value });
            }
        }

        public MockIdentityManagementService(IDictionary<string, string> userMappings)
            : this(userMappings.ToDictionary(kvp => kvp.Key, kvp => new MockTeamFoundationIdentity(kvp.Value, kvp.Key + "@domain.local") as ITeamFoundationIdentity))
        {
        }

        public MockIdentityManagementService(IDictionary<string, IEnumerable<ITeamFoundationIdentity>> accountMappings)
        {
            _accountNameMappings = accountMappings;
        }


        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return new MockIdentityDescriptor(identityType, identifier);
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(ICollection<IIdentityDescriptor> descriptors)
        {
            return descriptors
                    .Select(descriptor => Regex.Match(descriptor.Identifier, @".*\\(?<username>[^@]*)@").Groups["username"].Value)
                    .Where(username => username != null && _accountNameMappings.ContainsKey(username))
                    .Select(username => _accountNameMappings[username])
                    .SelectMany(s => s)
                    .ToList();
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
                            yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(value, _accountNameMappings[value]);
                        }
                    }
                    break;
                // Windows NT account name: domain\alias.
                case IdentitySearchFactor.AccountName:
                    foreach (var value in searchFactorValues)
                    {
                        yield return new KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>(
                            value,
                            Locate(identity => identity.UniqueName.StartsWith(value, StringComparison.OrdinalIgnoreCase)));
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
                        .Where(@t => predicate(@t.i))
                        .Select(@t => @t.i);
        }
    }
}
