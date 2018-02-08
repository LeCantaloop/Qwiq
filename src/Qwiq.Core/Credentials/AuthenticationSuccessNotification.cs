using Microsoft.VisualStudio.Services.Common;

namespace Qwiq.Credentials
{
    public class AuthenticationSuccessNotification : CredentialNotification
    {
        public ITeamProjectCollection TeamProjectCollection { get; }

        public AuthenticationSuccessNotification(VssCredentials credentials, ITeamProjectCollection teamProjectCollection)
            : base(credentials)
        {
            TeamProjectCollection = teamProjectCollection;
        }
    }
}