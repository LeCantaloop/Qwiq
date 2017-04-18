using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Credentials
{
    public class VssCredentialsComparer : GenericComparer<VssCredentials>
    {
        public static VssCredentialsComparer Instance => Nested.Instance;

        private VssCredentialsComparer()
        {
        }

        public override bool Equals(VssCredentials x, VssCredentials y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;


            return GenericComparer<WindowsCredential>.Default.Equals(x.Windows, y.Windows)
                   && GenericComparer<FederatedCredential>.Default.Equals(x.Federated, y.Federated);
        }

        public override int GetHashCode(VssCredentials obj)
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ (obj.Windows != null ? obj.Windows.GetHashCode() : 0);
                hash = (13 * hash) ^ (obj.Federated != null ? obj.Federated.GetHashCode() : 0);

                return hash;
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
            // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly VssCredentialsComparer Instance = new VssCredentialsComparer();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}