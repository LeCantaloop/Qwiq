using System;

using Microsoft.VisualStudio.Services.Common;

namespace Qwiq.Credentials
{
    public class AuthenticationFailedNotification : CredentialNotification
    {
        public AuthenticationFailedNotification(VssCredentials credentials, Exception exception)
            : this(credentials)
        {
            Exception = exception;
        }

        public AuthenticationFailedNotification(VssCredentials credentials)
            : base(credentials)
        {
        }

        public AuthenticationFailedNotification(Exception exception)
            :base(null)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}