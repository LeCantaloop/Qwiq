using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.TeamFoundation.Client;

using WindowsCredential = Microsoft.TeamFoundation.Client.WindowsCredential;

namespace Microsoft.Qwiq.Credentials
{
    public static class CredentialsFactory
    {
        /// <summary>
        /// Depending on the specific setup of the TFS server, it may or may not accept credentials of specific type. To accommodate for that
        /// without making the configuration more complicated (by making the user explicitly set which type of credentials to use), we just
        /// provide a list of all the relevant credential types we support in order of priority, and when connecting to TFS, we can try them
        /// in order and just go with the first one that succeeds.
        /// </summary>
        public static IEnumerable<TfsCredentials> CreateCredentials(string username = null, string password = null, string accessToken = null)
        {
            return CreateCredentials(
                    new Lazy<string>(() => username),
                    new Lazy<string>(() => password),
                    new Lazy<string>(() => accessToken));
        }

        public static IEnumerable<TfsCredentials> CreateCredentials(Lazy<string> username, Lazy<string> password, Lazy<string> accessToken)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (accessToken == null) throw new ArgumentNullException(nameof(accessToken));

            return CreateCredentialsImpl(username, password, accessToken)
                   .Select(c => new TfsCredentials(c));
        }

        private static IEnumerable<TfsClientCredentials> CreateCredentialsImpl(
            Lazy<string> username,
            Lazy<string> password,
            Lazy<string> accessToken)
        {
            // First try OAuth, as this is our preferred method
            foreach (var c in GetOAuthCredentials(accessToken.Value))
            {
                yield return c;
            }

            // Next try Username/Password combinations
            foreach (var c in GetServiceIdentityCredentials(username.Value, password.Value))
            {
                yield return c;
            }

            // Next try PAT
            foreach (var c in GetServiceIdentityPatCredentials(password.Value))
            {
                yield return c;
            }

            // Next try basic credentials
            foreach (var c in GetBasicCredentials(username.Value, password.Value))
            {
                yield return c;
            }

            // User did not specify a username or a password, so use the process identity
            yield return new TfsClientCredentials(new WindowsCredential(false)) { AllowInteractive = false };

            // Use the Windows identity of the logged on user
            yield return new TfsClientCredentials(true);
        }

        private static IEnumerable<TfsClientCredentials> GetBasicCredentials(
            string username = null,
            string password = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) yield break;

            yield return new TfsClientCredentials(new BasicAuthCredential(new NetworkCredential(username, password))) { AllowInteractive = false };
        }

        private static IEnumerable<TfsClientCredentials> GetServiceIdentityCredentials(
            string username = null,
            string password = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) yield break;

            yield return new TfsClientCredentials(new AadCredential(username, password)) { AllowInteractive = false };
            yield return new TfsClientCredentials(new WindowsCredential(new NetworkCredential(username, password))) { AllowInteractive = false };
        }

        private static IEnumerable<TfsClientCredentials> GetServiceIdentityPatCredentials(string password = null)
        {
            if (string.IsNullOrEmpty(password)) yield break;

            yield return new TfsClientCredentials(new BasicAuthCredential(new NetworkCredential(string.Empty, password))) { AllowInteractive = false };
        }

        private static IEnumerable<TfsClientCredentials> GetOAuthCredentials(string accessToken = null)
        {
            if (string.IsNullOrEmpty(accessToken)) yield break;

            yield return new TfsClientCredentials(new OAuthTokenCredential(accessToken));

            // TODO: Request token directly
        }
    }
}

