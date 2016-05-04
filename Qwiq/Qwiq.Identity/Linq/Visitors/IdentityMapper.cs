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

        private string GetDisplayName(string alias)
        {
            var identity = _identityManagementService.GetIdentityForAlias(alias, _tenantId, _domains);
            return identity?.DisplayName ?? alias;
        }

        public object Map(object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                return GetDisplayName(stringValue);
            }

            var stringArray = value as IEnumerable<string>;
            if (stringArray != null)
            {
                return stringArray.Select(GetDisplayName);
            }

            return value;
        }
    }
}