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

            var alias = identity.GetAttribute(IdentityAttributeTags.AccountName, null) ?? GetUserAlias(identity.Descriptor);

            if (!string.IsNullOrEmpty(alias))
            {
                return alias;
            }

            var uniqueName = identity.UniqueName;
            var uniqueNameSplit = uniqueName.Split('\\');
            return uniqueNameSplit.Length == 2
                ? uniqueNameSplit[1]
                : null;
        }

        public static string GetUserAlias(this IIdentityDescriptor descriptor)
        {
            if (!descriptor.Identifier.Contains("@")) return null;
            var identifier = descriptor.Identifier;
            var identifierSplit = identifier.Split('\\');

            if (identifierSplit.Length == 2)
            {
                return identifierSplit[1].Split('@')[0];
            }

            return null;
        }
    }
}
