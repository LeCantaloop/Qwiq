using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Identity
{

    /// <summary>
    /// Converts a <see cref="string"/> representing an alias to a user principal name.
    /// </summary>
    /// <seealso cref="IIdentityValueConverter" />
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
            IIdentityManagementService identityManagementService,
            string tenantId,
            params string[] domains)
        {
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
        public object Map(object value)
        {
            if (value is string stringValue) return GetIdentityNames(stringValue).Single();

            if (value is IEnumerable<string> stringArray) return GetIdentityNames(stringArray.ToArray());

            return value;
        }

        private string[] GetIdentityNames(params string[] aliases)
        {
            var identities = _identityManagementService.GetIdentityForAliases(aliases.ToList(), _tenantId, _domains);
            return identities.Select(i => i.Value.GetIdentityName() ?? i.Key).ToArray();
        }
    }
}