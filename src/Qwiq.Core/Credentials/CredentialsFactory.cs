using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;

namespace Microsoft.Qwiq.Credentials
{
    /// <summary>
    ///     Depending on the specific setup of the TFS server, it may or may not accept credentials of specific type. To
    ///     accommodate for that
    ///     without making the configuration more complicated (by making the user explicitly set which type of credentials to
    ///     use), we just
    ///     provide a list of all the relevant credential types we support in order of priority, and when connecting to TFS, we
    ///     can try them
    ///     in order and just go with the first one that succeeds.
    /// </summary>
    public static class CredentialsFactory
    {
        [Obsolete(
            "This method is deprecated and will be removed in a future release. See AuthenticationOptions instead.",
            false)]
        public static IEnumerable<TfsCredentials> CreateCredentials(
            string username = null,
            string password = null,
            string accessToken = null)
        {
            return CreateCredentials(
                new Lazy<string>(() => username),
                new Lazy<string>(() => password),
                new Lazy<string>(() => accessToken));
        }

        [Obsolete(
            "This method is deprecated and will be removed in a future release. See AuthenticationOptions instead.",
            false)]
        public static IEnumerable<TfsCredentials> CreateCredentials(
            Lazy<string> username = null,
            Lazy<string> password = null,
            Lazy<string> accessToken = null)
        {
            if (username == null) username = new Lazy<string>(() => string.Empty);
            if (password == null) password = new Lazy<string>(() => string.Empty);
            if (accessToken == null) accessToken = new Lazy<string>(() => string.Empty);

            return CreateCredentialsImpl(username, password, accessToken).Select(c => new TfsCredentials(c));
        }

        private static IEnumerable<VssCredentials> CreateCredentialsImpl(
            Lazy<string> username,
            Lazy<string> password,
            Lazy<string> accessToken)
        {
            // First try OAuth, as this is our preferred method
            foreach (var c in GetOAuthCredentials(accessToken.Value)) yield return c;

            // Next try Username/Password combinations
            foreach (var c in GetServiceIdentityCredentials(username.Value, password.Value)) yield return c;

            // Next try PAT
            foreach (var c in GetServiceIdentityPatCredentials(password.Value)) yield return c;

            // Next try basic credentials
            foreach (var c in GetBasicCredentials(username.Value, password.Value)) yield return c;

            // User did not specify a username or a password, so use the process identity
            yield return new VssClientCredentials(new WindowsCredential(false))
            {
                Storage = new VssClientCredentialStorage(),
                PromptType = CredentialPromptType.DoNotPrompt
            };

            // Use the Windows identity of the logged on user
            yield return new VssClientCredentials(true)
            {
                Storage = new VssClientCredentialStorage(),
                PromptType = CredentialPromptType.PromptIfNeeded
            };
        }

        internal static IEnumerable<VssCredentials> GetBasicCredentials(string username = null, string password = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) yield break;

            yield return new VssCredentials(new VssBasicCredential(username, password))
            {
                PromptType = CredentialPromptType.DoNotPrompt
            };
        }

        internal static IEnumerable<VssCredentials> GetOAuthCredentials(string accessToken = null)
        {
            if (string.IsNullOrEmpty(accessToken)) yield break;

            yield return new VssCredentials(new VssOAuthAccessTokenCredential(accessToken))
            {
                PromptType = CredentialPromptType.DoNotPrompt,
                Storage = new VssClientCredentialStorage()
            };
        }

        internal static IEnumerable<VssCredentials> GetServiceIdentityCredentials(
            string username = null,
            string password = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) yield break;

            yield return new VssCredentials(new VssAadCredential(username, password))
            {
                PromptType = CredentialPromptType.DoNotPrompt
            };
            yield return new VssCredentials(
                new WindowsCredential(new NetworkCredential(username, password)),
                CredentialPromptType.DoNotPrompt);
        }

        internal static IEnumerable<VssCredentials> GetServiceIdentityPatCredentials(string password = null)
        {
            if (string.IsNullOrEmpty(password)) yield break;

            yield return new VssCredentials(new VssBasicCredential(new NetworkCredential(string.Empty, password)))
            {
                PromptType = CredentialPromptType.DoNotPrompt
            };
        }
    }
}