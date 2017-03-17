using System.Diagnostics;

using Microsoft.TeamFoundation.Client;

namespace Microsoft.Qwiq.Credentials
{
    [DebuggerStepThrough]
    public sealed class TfsCredentials
    {
        public TfsCredentials(TfsClientCredentials credentials)
        {
            Credentials = credentials;
        }

        internal TfsClientCredentials Credentials { get; private set; }
    }
}