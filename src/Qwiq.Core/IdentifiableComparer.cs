namespace Microsoft.Qwiq
{
    public class IdentifiableComparer : GenericComparer<IIdentifiable<int>>
    {
        public static IdentifiableComparer Instance => Nested.Instance;

        private IdentifiableComparer()
        {
            
        }

        public override bool Equals(IIdentifiable<int> x, IIdentifiable<int> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Id == y.Id;
        }

        public override int GetHashCode(IIdentifiable<int> obj)
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