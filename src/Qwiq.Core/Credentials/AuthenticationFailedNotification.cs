using System;

namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationFailedNotification : CredentialNotification
    {
        public AuthenticationFailedNotification(TfsCredentials credentials, Exception exception)
            : this(credentials)
        {
            Exception = exception;
        }

        public AuthenticationFailedNotification(TfsCredentials credentials)
            : base(credentials)
        {

        }

        public Exception Exception { get; set; }
    }
}