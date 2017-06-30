namespace Microsoft.Qwiq
{
    internal class WorkItemComparer : GenericComparer<IWorkItem>
    {
        private WorkItemComparer()
        {
        }

        internal new static WorkItemComparer Default => Nested.Instance;

        public override bool Equals(IWorkItem x, IWorkItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return Comparer.Identifiable.Equals(x, y);
        }

        public override int GetHashCode(IWorkItem obj)
        {
            return IdentifiableComparer.Default.GetHashCode(obj);
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
}