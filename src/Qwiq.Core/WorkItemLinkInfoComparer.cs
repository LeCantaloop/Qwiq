namespace Microsoft.Qwiq
{
    public class WorkItemLinkInfoComparer : GenericComparer<IWorkItemLinkInfo>
    {
        internal new static WorkItemLinkInfoComparer Default => Nested.Instance;

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
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ obj.SourceId.GetHashCode();
                hash = (13 * hash) ^ obj.TargetId.GetHashCode();

                return hash;
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
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