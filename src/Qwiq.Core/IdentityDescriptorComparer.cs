using System;

namespace Microsoft.Qwiq
{
    public class IdentityDescriptorComparer : GenericComparer<IIdentityDescriptor>
    {
        public static IdentityDescriptorComparer Instance => Nested.Instance;

        public override bool Equals(IIdentityDescriptor x, IIdentityDescriptor y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Identifier, y.Identifier, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(x.IdentityType, y.IdentityType, StringComparison.OrdinalIgnoreCase);
        }

        

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
            // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly IdentityDescriptorComparer Instance = new IdentityDescriptorComparer();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}