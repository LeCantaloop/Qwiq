using System.Collections.Generic;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace Microsoft.IE.Qwiq.Credentials
{
    public static class CredentialsFactory
    {
        public static IEnumerable<TfsCredentials> CreateCredentials(string username = null, string password = null)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Service Identity
                yield return  new TfsCredentials(new TfsClientCredentials(new SimpleWebTokenCredential(username, password)));
                yield return new TfsCredentials(new TfsClientCredentials(new WindowsCredential(new NetworkCredential(username, password))));

                // PAT
                yield return new TfsCredentials(new TfsClientCredentials(new BasicAuthCredential(new NetworkCredential("", password))) { AllowInteractive = false });

                // Basic
                yield return new TfsCredentials(new TfsClientCredentials(new BasicAuthCredential(new NetworkCredential(username, password))) { AllowInteractive = false });
            }

            yield return new TfsCredentials(new TfsClientCredentials(new WindowsCredential(false)) { AllowInteractive = false });

            yield return new TfsCredentials(new TfsClientCredentials(true));
        }
    }
}
