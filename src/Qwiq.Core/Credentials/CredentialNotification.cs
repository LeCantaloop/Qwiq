using Microsoft.VisualStudio.Services.Common;

namespace Qwiq.Credentials
{
    public class CredentialNotification
    {
        public VssCredentials Credentials { get; }
        public CredentialNotification(VssCredentials credentials)
        {
            Credentials = credentials;
        }


    }
}