using System;
using System.Collections.Generic;

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
            AuthenticationType = authenticationType;
            ClientType = clientType;
            Notifications = new CredentialsNotifications();
            Uri = uri;
        }

        public CredentialsNotifications Notifications { get; set; }

        public Func<AuthenticationType, IEnumerable<TfsCredentials>> CreateCredentials { get; set; }
    }
}