using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Relatives.Tests.Mocks
{
    class MockIdentityMapper : IIdentityManagementService
    {
        private static readonly Dictionary<string, string> NameMap = new Dictionary<string, string>
        {
            { "Richard Murillo", "rimuri" },
            { "Matt Kotsenas", "mattkot" },
        };

        public string GetDisplayName(string alias)
        {
            return NameMap.SingleOrDefault(pair => pair.Value == alias).Key ?? alias;
        }

        public string TryGetAlias(string displayName)
        {
            return NameMap.ContainsKey(displayName) ? NameMap[displayName] : displayName;
        }

        public IEnumerable<ITeamFoundationIdentity> ReadIdentities(ICollection<IIdentityDescriptor> descriptors)
        {
            yield break;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<ITeamFoundationIdentity>>> ReadIdentities(IdentitySearchFactor searchFactor, ICollection<string> searchFactorValues)
        {
            yield break;
        }

        public IIdentityDescriptor CreateIdentityDescriptor(string identityType, string identifier)
        {
            return null;
        }
    }
}
