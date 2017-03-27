using System.Diagnostics;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    [DebuggerStepThrough]
    public sealed class TfsCredentials
    {
        public TfsCredentials(VssCredentials credentials)
        {
            Credentials = credentials;
        }



        internal VssCredentials Credentials { get; private set; }
    }
}