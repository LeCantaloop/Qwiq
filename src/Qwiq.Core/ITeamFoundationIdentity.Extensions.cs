using System;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq
{
    // ReSharper disable InconsistentNaming
    public static class ITeamFoundationIdentityExtensions
        // ReSharper restore InconsistentNaming
    {
        public static string GetUserAlias(this ITeamFoundationIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));

            var alias = identity.GetAttribute(IdentityAttributeTags.AccountName, null)
                        ?? new IdentityFieldValue(identity).Alias;

            return alias;
        }
    }
}