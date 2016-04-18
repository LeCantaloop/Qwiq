using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq;

namespace Qwiq.Identity.Tests.Mocks
{
    public class MockIdentityManagementService : IIdentityManagementService
    {
        private readonly IDictionary<string, IEnumerable<ITeamFoundationIdentity>> _identities;

        public MockIdentityManagementService() : this(new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>())
        {
        }

        public MockIdentityManagementService(IDictionary<string, IEnumerable<ITeamFoundationIdentity>> identities)
        {
            _identities = identities;
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(ICollection<IIdentityDescriptor> descriptors)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(IdentitySearchFactor searchFactor, ICollection<string> searchFactorValues)
        {
            return searchFactorValues.ToDictionary(k => k, k => _identities.ContainsKey(k) ? _identities[k] : Enumerable.Empty<ITeamFoundationIdentity>());
        }

        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            throw new NotImplementedException();
        }
    }
}