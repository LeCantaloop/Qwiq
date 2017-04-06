using System;
using System.Diagnostics;

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    [DebuggerStepThrough]
    public sealed class TfsCredentials : IEquatable<TfsCredentials>, IEquatable<VssCredentials>
    {
        public TfsCredentials(VssCredentials credentials)
        {
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        public bool Equals(TfsCredentials other)
        {
            return VssCredentialsComparer.Instance.Equals(Credentials, other?.Credentials);
        }

        public bool Equals(VssCredentials other)
        {
            return VssCredentialsComparer.Instance.Equals(Credentials, other);
        }

        public override bool Equals(object obj)
        {
            return VssCredentialsComparer.Instance.Equals(Credentials, (obj as TfsCredentials)?.Credentials);
        }

        public override int GetHashCode()
        {
            return VssCredentialsComparer.Instance.GetHashCode(Credentials);
        }

        internal VssCredentials Credentials { get; }

        public static implicit operator TfsCredentials(VssCredentials credentials)
        {
            return new TfsCredentials(credentials);
        }
    }
}