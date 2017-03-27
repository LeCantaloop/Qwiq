namespace Microsoft.Qwiq.Credentials
{
    public class CredentialNotification
    {
        public TfsCredentials Credentials { get; }
        public CredentialNotification(TfsCredentials credentials)
        {
            Credentials = credentials;
        }


    }
}