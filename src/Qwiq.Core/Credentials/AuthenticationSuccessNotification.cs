using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
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