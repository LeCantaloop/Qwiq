using System.Collections.Generic;
using System.Net;

using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;

namespace Qwiq.Credentials
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