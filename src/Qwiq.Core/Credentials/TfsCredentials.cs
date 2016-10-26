using Microsoft.TeamFoundation.Client;

namespace Microsoft.Qwiq.Credentials
{
    public sealed class TfsCredentials
    {
        internal TfsCredentials(TfsClientCredentials credentials)
        {
            Credentials = credentials;
        }

        internal TfsClientCredentials Credentials { get; private set; }
    }
}

