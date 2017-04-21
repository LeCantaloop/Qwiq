using System;

namespace Microsoft.Qwiq
{
    public class TeamFoundationIdentityComparer : GenericComparer<ITeamFoundationIdentity>
    {
        internal new static TeamFoundationIdentityComparer Default => Nested.Instance;

        public override int GetHashCode(ITeamFoundationIdentity obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            return IdentityDescriptorComparer.Default.GetHashCode(obj.Descriptor);
        }

        public override bool Equals(ITeamFoundationIdentity x, ITeamFoundationIdentity y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return StringComparer.OrdinalIgnoreCase.Equals(x.UniqueName, y.UniqueName)
                && IdentityDescriptorComparer.Default.Equals(x.Descriptor, y.Descriptor);
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
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}