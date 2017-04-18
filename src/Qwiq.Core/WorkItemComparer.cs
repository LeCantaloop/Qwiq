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
}