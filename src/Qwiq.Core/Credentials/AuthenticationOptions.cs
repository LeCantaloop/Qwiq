using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationOptions
    {
        public Uri Uri { get; }

        public AuthenticationType AuthenticationType { get; }

        public AuthenticationOptions(string uri)
            : this(new Uri(uri, UriKind.Absolute))
        {
        }

        public AuthenticationOptions(Uri uri, AuthenticationType authenticationType = AuthenticationType.All)
        {
            AuthenticationType = authenticationType;
            Notifications = new CredentialsNotifications();
            Uri = uri;
        }

        public CredentialsNotifications Notifications { get; set; }

        public Func<AuthenticationType, IEnumerable<TfsCredentials>> CreateCredentials { get; set; }
    }
}