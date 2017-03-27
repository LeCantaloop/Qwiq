namespace Microsoft.Qwiq.Credentials
{
    public class AuthenticationSuccessNotification : CredentialNotification
    {
        public ITfsTeamProjectCollection TeamProjectCollection { get; }

        public AuthenticationSuccessNotification(TfsCredentials credentials, ITfsTeamProjectCollection teamProjectCollection)
            : base(credentials)
        {
            TeamProjectCollection = teamProjectCollection;
        }
    }
}