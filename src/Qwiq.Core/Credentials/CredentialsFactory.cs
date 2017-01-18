using System.Collections.Generic;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace Microsoft.Qwiq.Credentials
{
    public static class CredentialsFactory
    {
        public static IEnumerable<TfsCredentials> CreateCredentials(string username = null, string password = null)
        {
            if (!string.IsNullOrEmpty(password))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    yield return new TfsCredentials(new TfsClientCredentials(new AadCredential(username, password)) { AllowInteractive = false });

                    // Service Identity - Non Interactive
                    yield return  new TfsCredentials(new TfsClientCredentials(new SimpleWebTokenCredential(username, password)) {AllowInteractive = false});

                    // Try username and password. If user sent a PAT with a username, will work like a regular password
                    yield return new TfsCredentials(new TfsClientCredentials(new BasicAuthCredential(new NetworkCredential(username, password))) { AllowInteractive = false });
                }

                // PAT - user can specify a PAT with no username
                yield return new TfsCredentials(new TfsClientCredentials(new BasicAuthCredential(new NetworkCredential("", password))) { AllowInteractive = false });
            }

            // User did not specify a username or a password, so use the process identity
            yield return new TfsCredentials(new TfsClientCredentials(new WindowsCredential(false)) { AllowInteractive = false });

            // Use the Windows identity of the logged on user
            yield return new TfsCredentials(new TfsClientCredentials(true));
        }
    }
}

