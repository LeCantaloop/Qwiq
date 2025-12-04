using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Qwiq.Identity
{
    /// <summary>
    /// Converts a <see cref="string"/> representing an identity to an alias
    /// </summary>
    public class DisplayNameToAliasValueConverter : IdentityValueConverterBase
    {
        private static readonly IReadOnlyDictionary<string, object> Empty = new Dictionary<string, object>();
        private readonly IIdentityManagementService _identityManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameToAliasValueConverter"/> class.
        /// </summary>
        /// <param name="identityManagementService">The identity management service.</param>
        /// <exception cref="ArgumentNullException">identityManagementService</exception>
        public DisplayNameToAliasValueConverter(IIdentityManagementService identityManagementService)
        {
            Contract.Requires(identityManagementService != null);

            _identityManagementService = identityManagementService ?? throw new ArgumentNullException(nameof(identityManagementService));
        }

        public override IReadOnlyDictionary<string, object> Map(IEnumerable<string> values)
        {
            if (values == null) return Empty;
            var result = GetIdentityNames(values.ToArray());
            // Filter out null values to satisfy the non-nullable contract
            return result.Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value!, Comparer.OrdinalIgnoreCase);
        }

        private IDictionary<string, string[]> GetAliasesForDisplayNames(string[] displayNames)
        {
            if (displayNames == null) throw new ArgumentNullException(nameof(displayNames));

            return _identityManagementService.ReadIdentities(IdentitySearchFactor.DisplayName, displayNames)
                      .ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp
                                            .Value?.Where(
                                                         identity => identity != null
                                                                     && !identity.IsContainer
                                                                     && identity.UniqueUserId == IdentityConstants.ActiveUniqueId)
                                            .Select(i => i.GetUserAlias())
                                            .Where(alias => alias != null)
                                            .Cast<string>()
                                            .Distinct(StringComparer.OrdinalIgnoreCase)
                                            .ToArray() ?? Array.Empty<string>());
        }

        private Dictionary<string, object?> GetIdentityNames(params string[] displayNames)
        {
            return
                        GetAliasesForDisplayNames(displayNames)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp =>
                                {
                                    if (kvp.Value == null || kvp.Value.Length == 0) return null;
                                    if (kvp.Value.Length > 1)
                                    {
                                        throw new MultipleIdentitiesFoundException(kvp.Key, kvp.Value);
                                    }
                                    return (object?)kvp.Value[0];
                                },
                            Comparer.OrdinalIgnoreCase);
        }
    }
}