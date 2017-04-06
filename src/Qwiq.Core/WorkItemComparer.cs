namespace Microsoft.Qwiq
{
    public class WorkItemComparer : GenericComparer<IWorkItem>
    {
        private WorkItemComparer()
        {
        }

        public static WorkItemComparer Instance => Nested.Instance;

        public override bool Equals(IWorkItem x, IWorkItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return IdentifiableComparer.Instance.Equals(x, y);
        }

        public override int GetHashCode(IWorkItem obj)
        {
            return IdentifiableComparer.Instance.GetHashCode(obj);
        }

        private class Nested
        {
            internal static readonly WorkItemComparer Instance = new WorkItemComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }

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