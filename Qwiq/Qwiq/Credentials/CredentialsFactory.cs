using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;

namespace Microsoft.IE.Qwiq.Credentials
{
    public class CredentialsFactory
    {
        private CredentialsFactory()
        {
        }

        public static CredentialsFactory GetInstance()
        {
            return new CredentialsFactory();
        }

        public TfsCredentials CreateAadCredentials(string tfsResourceString, string tfsClientId, string authority,
            string username = null, string password = null)
        {
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                return new TfsCredentials(new AadCredential());
            }

            var credentials = new UserCredential(username, password);
            var authContext = new AuthenticationContext(authority);
            var token = new AadToken(authContext.AcquireToken(tfsResourceString, tfsClientId, credentials));

            return new TfsCredentials(new AadCredential(token));
        }

        public TfsCredentials CreateAcsCredentials(string username, string password)
        {
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                return new TfsCredentials(new TfsClientCredentials(true));
            }

            var credentials = new SimpleWebTokenCredential(username, password);
            return new TfsCredentials(new TfsClientCredentials(credentials));
        }
    }
}
