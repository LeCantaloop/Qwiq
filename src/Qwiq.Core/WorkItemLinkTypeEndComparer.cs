using System;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkTypeEndComparer : GenericComparer<IWorkItemLinkTypeEnd>
    {
        internal new static WorkItemLinkTypeEndComparer Default => Nested.Instance;

        public override bool Equals(IWorkItemLinkTypeEnd x, IWorkItemLinkTypeEnd y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.IsForwardLink == y.IsForwardLink
                   && x.ImmutableName.Equals(y.ImmutableName, StringComparison.OrdinalIgnoreCase)
                   && x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(IWorkItemLinkTypeEnd obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;

                hash = (13 * hash) ^ obj.Id.GetHashCode();
                hash = (13 * hash) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ImmutableName);

                return hash;
            }
        }

        // ReSharper disable ClassNeverInstantiated.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Nested
        // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemLinkTypeEndComparer Instance = new WorkItemLinkTypeEndComparer();
            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}