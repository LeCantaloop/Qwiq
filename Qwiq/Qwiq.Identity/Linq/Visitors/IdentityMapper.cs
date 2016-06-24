using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Identity.Linq.Visitors
{
    public class IdentityMapper : IIdentityMapper
    {
        private readonly IIdentityManagementService _identityManagementService;
        private readonly string _tenantId;
        private readonly string[] _domains;

        public IdentityMapper(IIdentityManagementService identityManagementService, string tenantId, params string[] domains)
        {
            _identityManagementService = identityManagementService;
            _tenantId = tenantId;
            _domains = domains;
        }

        private string[] GetDisplayNames(params string[] aliases)
        {
            var identities = _identityManagementService.GetIdentityForAliases(aliases.ToList(), _tenantId, _domains);
            return identities.Select(i => i.Value?.DisplayName ?? i.Key).ToArray();
        }

        public object Map(object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                return GetDisplayNames(stringValue).Single();
            }

            var stringArray = value as IEnumerable<string>;
            if (stringArray != null)
            {
                return GetDisplayNames(stringArray.ToArray());
            }

            return value;
        }
    }
}