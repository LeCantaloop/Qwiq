using System;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationFailedNotification : AuthenticationNotification, IAuthenticationFailedNotification
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

        public Exception Exception { get; set; }
    }
}