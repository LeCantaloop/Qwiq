using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq
{
    // ReSharper disable InconsistentNaming
    public static partial class Extensions
    // ReSharper restore InconsistentNaming
    {

        public static string GetUserAlias(this ITeamFoundationIdentity identity)
        {
            if (identity == null) return null;

            return identity.GetAttribute(IdentityAttributeTags.Alias, null)
                        ?? new IdentityFieldValue(identity).Alias;
        }

        public static string GetUserAccountName(this ITeamFoundationIdentity identity)
        {
            if (identity == null) return null;

            return identity.GetAttribute(IdentityAttributeTags.AccountName, null)
                   ?? new IdentityFieldValue(identity).AccountName;
        }

        public static string GetIdentityName(this ITeamFoundationIdentity identity)
        {
            if (identity == null) return null;

            return identity.GetAttribute(IdentityAttributeTags.AadUserPrincipalName, null) ?? new IdentityFieldValue(identity).IdentityName;
        }
    }
}