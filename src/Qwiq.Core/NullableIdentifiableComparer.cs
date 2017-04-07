namespace Microsoft.Qwiq
{
    public class NullableIdentifiableComparer : GenericComparer<IIdentifiable<int?>>
    {
        public static NullableIdentifiableComparer Instance => Nested.Instance;

        private NullableIdentifiableComparer()
        {

        }

        public override bool Equals(IIdentifiable<int?> x, IIdentifiable<int?> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Id == y.Id;
        }

        public override int GetHashCode(IIdentifiable<int?> obj)
        {
            unchecked
            {
                var hash = 27;

                hash = (13 * hash) ^ (obj.Id.HasValue ? obj.Id.GetHashCode() : 0);

                return hash;
            }
        }

        private class Nested
        {
            internal static readonly NullableIdentifiableComparer Instance = new NullableIdentifiableComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}