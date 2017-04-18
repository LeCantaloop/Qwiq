using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationOptions : IAuthenticationOptions
    {
        public AuthenticationOptions(
            string uri,
            AuthenticationType authenticationType = AuthenticationType.All,
            ICredentialsNotifications notifications = null)
            : this(new Uri(uri, UriKind.Absolute), authenticationType, notifications)
        {
        }

        public AuthenticationOptions(
            Uri uri,
            AuthenticationType authenticationType = AuthenticationType.All,
            ICredentialsNotifications notifications = null)
        {
            AuthenticationType = authenticationType;
            Notifications = notifications ?? new CredentialsNotifications();
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public AuthenticationType AuthenticationType { get; }

        public Func<AuthenticationType, IEnumerable<VssCredentials>> CreateCredentials { get; set; }

        public IEnumerable<VssCredentials> Credentials
        {
            get
            {
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.OAuth)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.PersonalAccessToken)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.Basic)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.Windows)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.Anonymous)) yield return credential;
            }
        }

        public ICredentialsNotifications Notifications { get; }

        public Uri Uri { get; }

        private static IEnumerable<VssCredentials> EnumerateCredentials(
            AuthenticationOptions authenticationOptions,
            AuthenticationType authenticationType)
        {
            if (!authenticationOptions.AuthenticationType.HasFlag(authenticationType)) yield break;

            var credentials = Enumerable.Empty<VssCredentials>();

            try
            {
                credentials = authenticationOptions.CreateCredentials(authenticationType);
            }
            catch (Exception e)
            {
                authenticationOptions.Notifications.AuthenticationFailed(new AuthenticationFailedNotification(null) { Exception = e });
                throw;
            }

            foreach (var credential in credentials) yield return credential;
        }
    }
}