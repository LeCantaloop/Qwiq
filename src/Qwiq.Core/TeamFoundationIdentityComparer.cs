using System;

namespace Microsoft.Qwiq
{
    public class TeamFoundationIdentityComparer : GenericComparer<ITeamFoundationIdentity>
    {
        public static TeamFoundationIdentityComparer Instance => Nested.Instance;

        public override int GetHashCode(ITeamFoundationIdentity obj)
        {
            unchecked
            {
                if (obj.Descriptor == null) return 0;
                return obj.Descriptor.GetHashCode();
            }
        }

        public override bool Equals(ITeamFoundationIdentity x, ITeamFoundationIdentity y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return (string.Equals(x.UniqueName, y.UniqueName, StringComparison.OrdinalIgnoreCase));
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
            // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly TeamFoundationIdentityComparer Instance = new TeamFoundationIdentityComparer();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}