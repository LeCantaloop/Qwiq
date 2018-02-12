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
        public static string GetIdentityName(this ITeamFoundationIdentity identity)
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
        public static string GetUserAccountName(this ITeamFoundationIdentity identity)
        {
            if (identity == null) return null;

            return identity.GetAttribute(IdentityAttributeTags.AccountName, null) ?? new IdentityFieldValue(identity).AccountName;
        }

        /// <summary>
        ///     Gets the user account (logon) name from the specified <see cref="ITeamFoundationIdentity" /> instance.
        /// </summary>
        /// <param name="identity">An instance of <see cref="ITeamFoundationIdentity" />.</param>
        /// <returns>
        ///     <see cref="IdentityFieldValue.LogonName" />
        /// </returns>
        public static string GetUserAlias(this ITeamFoundationIdentity identity)
        {
            return identity == null ? null : new IdentityFieldValue(identity).LogonName;
        }
    }
}