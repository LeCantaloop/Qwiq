using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    public abstract class AuthenticationNotification : IAuthenticationNotification
    {
        public VssCredentials Credentials { get; }

        protected AuthenticationNotification(VssCredentials credentials)
        {
            Credentials = credentials;
        }


    }
}