using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;

namespace Microsoft.IE.Qwiq.Credentials
{
    public static class CredentialsFactory
    {
        [Obsolete("The AAD resource, Client ID, and Authority are no longer needed. Use the other overload of this method instead.")]
        public static TfsCredentials CreateAadCredentials(
            string tfsResourceString,
            string tfsClientId,
            string authority,
            string username = null,
            string password = null)
        {
            return CreateAadCredentials(username, password);
        }

        public static TfsCredentials CreateAadCredentials(string username = null, string password = null)
        {
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                return new TfsCredentials(new AadCredential());
            }

            return new TfsCredentials(new AadCredential(username, password));
        }

        public static TfsCredentials CreateAcsCredentials(string username, string password)
        {
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                return new TfsCredentials(new TfsClientCredentials(true));
            }

            var credentials = new SimpleWebTokenCredential(username, password);
            return new TfsCredentials(new TfsClientCredentials(credentials));
        }
    }
}
