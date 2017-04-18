namespace Microsoft.Qwiq
{
    public class WorkItemLinkInfoComparer : GenericComparer<IWorkItemLinkInfo>
    {
        public static WorkItemLinkInfoComparer Instance => Nested.Instance;

        public override bool Equals(IWorkItemLinkInfo x, IWorkItemLinkInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.SourceId == y.SourceId
                   && x.TargetId == y.TargetId;
        }

        public override int GetHashCode(IWorkItemLinkInfo obj)
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ obj.SourceId.GetHashCode();
                hash = (13 * hash) ^ obj.TargetId.GetHashCode();

                return hash;
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
            // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemLinkInfoComparer Instance = new WorkItemLinkInfoComparer();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}