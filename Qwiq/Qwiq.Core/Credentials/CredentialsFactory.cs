using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace Microsoft.IE.Qwiq.Credentials
{
    public static class CredentialsFactory
    {
        public static TfsCredentials CreateAadCredentials()
        {
            return new TfsCredentials(new TfsClientCredentials(true));
        }

        public static TfsCredentials CreateAadCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            return new TfsCredentials(new WindowsCredential(new NetworkCredential(username, password)));
        }
    }
}
