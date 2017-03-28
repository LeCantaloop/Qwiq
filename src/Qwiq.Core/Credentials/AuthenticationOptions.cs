using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationOptions
    {
        public Uri Uri { get; }

        public AuthenticationType AuthenticationType { get; }

        public ClientType ClientType { get; }

        public AuthenticationOptions(string uri)
            : this(new Uri(uri, UriKind.Absolute))
        {
        }

        public AuthenticationOptions(Uri uri, AuthenticationType authenticationType = AuthenticationType.All, ClientType clientType = ClientType.Default)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));


            AuthenticationType = authenticationType;
            ClientType = clientType;
            Notifications = new CredentialsNotifications();
            Uri = uri;
        }

        public CredentialsNotifications Notifications { get; set; }

        public Func<AuthenticationType, IEnumerable<TfsCredentials>> CreateCredentials { get; set; }

        public IEnumerable<TfsCredentials> Credentials
        {
            get
            {
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.OAuth)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.PersonalAccessToken))
                    yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.Basic)) yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.Windows))
                    yield return credential;
                foreach (var credential in EnumerateCredentials(this, AuthenticationType.Anonymous))
                    yield return credential;
            }
        }

        private static IEnumerable<TfsCredentials> EnumerateCredentials(
            AuthenticationOptions authenticationOptions,
            AuthenticationType authenticationType)
        {
            if (!authenticationOptions.AuthenticationType.HasFlag(authenticationType)) yield break;

            var credentials = Enumerable.Empty<TfsCredentials>();

            try
            {
                credentials = authenticationOptions.CreateCredentials(authenticationType);
            }
            catch (Exception e)
            {
                authenticationOptions.Notifications.AuthenticationFailed(
                    new AuthenticationFailedNotification(null) { Exception = e });
                throw;
            }

            foreach (var credential in credentials) yield return credential;
        }
    }
}
