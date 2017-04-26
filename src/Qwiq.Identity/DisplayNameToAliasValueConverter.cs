using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Identity
{
    /// <summary>
    /// Converts a <see cref="string"/> representing an identity to an alias
    /// </summary>
    /// <seealso cref="IIdentityValueConverter" />
    public class DisplayNameToAliasValueConverter : IIdentityValueConverter
    {
        private readonly IIdentityManagementService _identityManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameToAliasValueConverter"/> class.
        /// </summary>
        /// <param name="identityManagementService">The identity management service.</param>
        /// <exception cref="ArgumentNullException">identityManagementService</exception>
        public DisplayNameToAliasValueConverter(IIdentityManagementService identityManagementService)
        {
            _identityManagementService = identityManagementService ?? throw new ArgumentNullException(nameof(identityManagementService));
        }

        /// <summary>
        /// Converts the specified <paramref name="value" /> to an <see cref="T:Dictionary{string, string}" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="T:Dictionary{string, object}" /> instance whose key is the <paramref name="value"/> and value is is equivalent to the value of <paramref name="value" />.</returns>
        public object Map(object value)
        {
            if (value is string stringValue) return GetIdentityNames(stringValue);

            if (value is IEnumerable<string> stringArray) return GetIdentityNames(stringArray.ToArray());

            return value;
        }

        private Dictionary<string, string> GetIdentityNames(params string[] displayNames)
        {
            return
                        GetAliasesForDisplayNames(displayNames)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp =>
                                {
                                    if (kvp.Value == null) return null;
                                    var retval = kvp.Value.FirstOrDefault();
                                    if (kvp.Value.Length > 1)
                                    {
                                        Trace.TraceWarning("Multiple identities found matching '{0}':\r\n\r\n- {1}\r\n\r\nChoosing '{2}'.", kvp.Key, string.Join("\r\n- ", kvp.Value), retval);
                                    }
                                    return retval;
                                },
                            Comparer.OrdinalIgnoreCase);
        }

        private IDictionary<string, string[]> GetAliasesForDisplayNames(string[] displayNames)
        {
            if (displayNames == null) throw new ArgumentNullException(nameof(displayNames));

            return _identityManagementService.ReadIdentities(IdentitySearchFactor.DisplayName, displayNames)
                      .ToDictionary(
                                    kvp => kvp.Key,
                                    kvp => kvp
                                            .Value.Where(
                                                         identity => identity != null
                                                                     && !identity.IsContainer
                                                                     && identity.UniqueUserId == IdentityConstants.ActiveUniqueId)
                                            .Select(i => i.GetUserAlias())
                                            .Distinct(StringComparer.OrdinalIgnoreCase)
                                            .ToArray());
        }
    }
}
