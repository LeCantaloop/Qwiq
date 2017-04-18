using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationSuccessNotification : AuthenticationNotification, IAuthenticationSuccessNotification
    {
        public ITfsTeamProjectCollection TeamProjectCollection { get; }

        public AuthenticationSuccessNotification(VssCredentials credentials, ITfsTeamProjectCollection teamProjectCollection)
            : base(credentials)
        {
            TeamProjectCollection = teamProjectCollection;
        }
    }
}