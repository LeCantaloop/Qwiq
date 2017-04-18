using System.Collections.Generic;

using Microsoft.Qwiq.Mapper;

namespace Microsoft.Qwiq.Identity.Mapper
{


    internal class IdentifiableComparer : GenericComparer<IIdentifiable>
    {
        private IdentifiableComparer()
        {
            
        }

        public static IdentifiableComparer Instance => Nested.Instance;

        public override bool Equals(IIdentifiable x, IIdentifiable y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Id == y.Id;
        }

        public override int GetHashCode(IIdentifiable obj)
        {
            unchecked
            {


                var hash = 27;

                hash = (13 * hash) ^ obj.Id.GetHashCode();

                return hash;
            }
        }

        private class Nested
        {
            internal static readonly IdentifiableComparer Instance = new IdentifiableComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}
