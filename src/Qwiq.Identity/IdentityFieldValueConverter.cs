using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwiq.Identity
{
    public class IdentityFieldValueConverter : IdentityValueConverterBase
    {
        private static readonly IReadOnlyDictionary<string, object> Empty = new Dictionary<string, object>();
        [NotNull] private readonly IIdentityManagementService _identityManagementService;

        public IdentityFieldValueConverter(
            [NotNull] IIdentityManagementService identityManagementService)
        {
            _identityManagementService = identityManagementService ?? throw new ArgumentNullException(nameof(identityManagementService));
        }


        public override IReadOnlyDictionary<string, object> Map(IEnumerable<string> values)
        {
            if (values == null) return Empty;

            var identities = _identityManagementService.ReadIdentities(IdentitySearchFactor.DisplayName, values);
            var retval = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var identity in identities)
            {
                var c = identity.Value?.Count();
                if (c.GetValueOrDefault(0) == 0)
                {
                    retval.Add(identity.Key, new IdentityFieldValue(identity.Key));
                    continue;
                }

                if (c > 1)
                {
                    var m =
                        $"Multiple identities found matching '{identity.Key}'. Please specify one of the following identities:{string.Join("\r\n- ", identity.Value)}";

                    throw new MultipleIdentitiesFoundException(m);
                }

                var v = new IdentityFieldValue(identity.Value.FirstOrDefault());
                retval.Add(identity.Key, v);
            }

            return retval;
        }
    }
}