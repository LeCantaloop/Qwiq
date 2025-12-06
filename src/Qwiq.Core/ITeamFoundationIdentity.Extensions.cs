using Microsoft.VisualStudio.Services.Common;

namespace Qwiq
{
    // ReSharper disable InconsistentNaming
    public static partial class Extensions
    // ReSharper restore InconsistentNaming
    {
        /// <summary>
        ///     Gets the identity name from the specified <see cref="ITeamFoundationIdentity" /> instance.
        /// </summary>
        /// <param name="identity">An instance of <see cref="ITeamFoundationIdentity" />.</param>
        /// <returns>
        ///     The value of the <see cref="IdentityAttributeTags.AccountName" /> property of the identity if it is not null;
        ///     otherwise, <see cref="IdentityFieldValue.IdentityName" />.
        /// </returns>
        public static string? GetIdentityName(this ITeamFoundationIdentity? identity)
        {
            if (identity == null) return null;

            return identity.GetAttribute(IdentityAttributeTags.AccountName, null) ?? new IdentityFieldValue(identity).IdentityName;
        }

        /// <summary>
        ///     Gets the user account name from the specified <see cref="ITeamFoundationIdentity" /> instance.
        /// </summary>
        /// <param name="identity">An instance of <see cref="ITeamFoundationIdentity" />.</param>
        /// <returns>
        ///     The value of the <see cref="IdentityAttributeTags.AccountName" /> property; otherwise,
        ///     <see cref="IdentityFieldValue.AccountName" />.
        /// </returns>
        public static string? GetUserAccountName(this ITeamFoundationIdentity? identity)
        {
            if (identity == null) return null;

            return identity.GetAttribute(IdentityAttributeTags.AccountName, null) ?? new IdentityFieldValue(identity).AccountName;
        }

        /// <summary>
        ///     Gets the user account (logon) name from the specified <see cref="ITeamFoundationIdentity" /> instance.
        /// </summary>
        /// <param name="identity">An instance of <see cref="ITeamFoundationIdentity" />.</param>
        /// <returns>
        ///     The logon name extracted from the identity descriptor, or from the <see cref="ITeamFoundationIdentity.UniqueName"/> if available.
        /// </returns>
        public static string? GetUserAlias(this ITeamFoundationIdentity? identity)
        {
            if (identity == null) return null;

            // First try to get the logon name from the IdentityFieldValue (uses descriptor)
            var identityFieldValue = new IdentityFieldValue(identity);
            if (!string.IsNullOrEmpty(identityFieldValue.LogonName))
            {
                return identityFieldValue.LogonName;
            }

            // Fallback: try to extract from UniqueName (could be "alias", "DOMAIN\alias", or "alias@domain.com")
            if (!string.IsNullOrEmpty(identity.UniqueName))
            {
                var uniqueName = identity.UniqueName;

                // Handle DOMAIN\alias format
                var backslashIndex = uniqueName.IndexOf('\\');
                if (backslashIndex >= 0 && backslashIndex < uniqueName.Length - 1)
                {
                    // Extract the part after the backslash (the account name)
                    var afterBackslash = uniqueName.Substring(backslashIndex + 1);

                    // Handle edge case where after backslash starts with @ (e.g., "DOMAIN\@something")
                    // This is not a valid email, so return the whole thing after backslash
#if NETFRAMEWORK || NETSTANDARD2_0
                    if (afterBackslash.StartsWith("@", StringComparison.Ordinal))
#else
                    if (afterBackslash.StartsWith('@'))
#endif
                    {
                        return afterBackslash;
                    }

                    // If the account name is an email, extract just the alias part
                    var atIndex = afterBackslash.IndexOf('@');
                    if (atIndex > 0)
                    {
                        return afterBackslash.Substring(0, atIndex);
                    }

                    return afterBackslash;
                }

                // Handle email format (alias@domain.com)
                var emailAtIndex = uniqueName.IndexOf('@');
                if (emailAtIndex > 0)
                {
                    return uniqueName.Substring(0, emailAtIndex);
                }

                // If no backslash or @, the UniqueName might be the alias itself
                // But only return it if it doesn't look like a display name (contains spaces)
                if (!uniqueName.Contains(' '))
                {
                    return uniqueName;
                }
            }

            return null;
        }
    }
}