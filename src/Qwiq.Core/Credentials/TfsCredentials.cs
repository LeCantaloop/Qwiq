using System;
using System.Diagnostics;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    [Obsolete("This type will be removed in a future release. Use VssCredentials instead.")]
    [DebuggerStepThrough]
    public sealed class TfsCredentials
    {
        public TfsCredentials(VssCredentials credentials)
        {
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        internal VssCredentials Credentials { get; }

        public static implicit operator TfsCredentials(VssCredentials credentials)
        {
            return new TfsCredentials(credentials);
        }
    }
}