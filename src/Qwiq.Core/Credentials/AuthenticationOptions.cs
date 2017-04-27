using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationOptions
    {
        private readonly Func<AuthenticationTypes, IEnumerable<VssCredentials>> _createCredentials;

        public AuthenticationOptions(string uri)
            : this(new Uri(uri, UriKind.Absolute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationOptions"/> class.
        /// </summary>
        /// <param name="uri">The URI of the Team Foundation Server, including the project collection.</param>
        /// <param name="authenticationTypes">The authentication types to use against the server.</param>
        /// <param name="clientType">Type of the client.</param>
        public AuthenticationOptions(
            string uri,
            AuthenticationTypes authenticationTypes = AuthenticationTypes.All)
            : this(new Uri(uri, UriKind.Absolute), authenticationTypes)
        {
        }

        public AuthenticationOptions(Uri uri)
            : this(uri, AuthenticationTypes.All)
        {
        }

        public AuthenticationOptions(
            Uri uri,
            AuthenticationTypes authenticationTypes,
            Func<AuthenticationTypes, IEnumerable<VssCredentials>> credentialsFactory = null)
        {
            AuthenticationTypes = authenticationTypes;
            Notifications = new CredentialsNotifications();
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _createCredentials = credentialsFactory ?? CredentialsFactory;
        }

        public AuthenticationTypes AuthenticationTypes { get; }



        public IEnumerable<VssCredentials> Credentials
        {
            get
            {
                foreach (var credential in EnumerateCredentials(this, AuthenticationTypes.OpenAuthorization)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationTypes.PersonalAccessToken)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationTypes.Basic)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationTypes.Windows)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationTypes.None)) yield return credential;
            }
        }

        public CredentialsNotifications Notifications { get; set; }

        public Uri Uri { get; }

        private static IEnumerable<VssCredentials> CredentialsFactory(AuthenticationTypes t)
        {
            if (t.HasFlag(AuthenticationTypes.OpenAuthorization))
                foreach (var cred in Qwiq.Credentials.CredentialsFactory.GetOAuthCredentials()) yield return cred;

            if (t.HasFlag(AuthenticationTypes.PersonalAccessToken))
                foreach (var cred in Qwiq.Credentials.CredentialsFactory.GetServiceIdentityPatCredentials()) yield return cred;

            if (t.HasFlag(AuthenticationTypes.Windows))
                foreach (var cred in Qwiq.Credentials.CredentialsFactory.GetServiceIdentityCredentials()) yield return cred;

            if (t.HasFlag(AuthenticationTypes.Basic))
                foreach (var cred in Qwiq.Credentials.CredentialsFactory.GetBasicCredentials()) yield return cred;

            if (t.HasFlag(AuthenticationTypes.Windows))
            {
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
        }

        private static IEnumerable<VssCredentials> EnumerateCredentials(
            AuthenticationOptions authenticationOptions,
            AuthenticationTypes authenticationTypes)
        {
            if (!authenticationOptions.AuthenticationTypes.HasFlag(authenticationTypes)) yield break;

            IEnumerable<VssCredentials> credentials;

            try
            {
                credentials = authenticationOptions._createCredentials(authenticationTypes);
            }
            catch (Exception e)
            {
                authenticationOptions.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(e));
                throw;
            }

            foreach (var credential in credentials) yield return credential;
        }
    }
}