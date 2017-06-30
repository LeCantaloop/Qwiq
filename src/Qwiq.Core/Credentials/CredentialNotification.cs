using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
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