using System;

namespace Microsoft.Qwiq
{
    public static class ITeamFoundationIdentityExtensions
    {

        public static string GetUserAlias(this ITeamFoundationIdentity identity)
        {
            if (identity == null) throw new ArgumentNullException("identity");

            if (identity.Descriptor.Identifier.Contains("@"))
            {
                var identifier = identity.Descriptor.Identifier;
                var identifierSplit = identifier.Split('\\');

                if (identifierSplit.Length == 2)
                {
                    return identifierSplit[1].Split('@')[0];
                }
            }
            else
            {
                var uniqueName = identity.UniqueName;
                var uniqueNameSplit = uniqueName.Split('\\');
                if (uniqueNameSplit.Length == 2)
                {
                    return uniqueNameSplit[1];
                }
            }

            return null;
        }
    }
}
