namespace Microsoft.Qwiq
{
    public class IdentifiableComparer : GenericComparer<IIdentifiable<int>>
    {
        internal new static IdentifiableComparer Default => Nested.Instance;

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
            if (ReferenceEquals(obj, null)) return 0;

            return 351 ^ obj.Id.GetHashCode();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Nested
        {
            internal static readonly IdentifiableComparer Instance = new IdentifiableComparer();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}