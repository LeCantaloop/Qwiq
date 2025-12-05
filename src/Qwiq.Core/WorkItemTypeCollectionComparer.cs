using System.Linq;

namespace Qwiq
{
    internal class WorkItemTypeCollectionComparer : GenericComparer<IWorkItemTypeCollection>
    {
        internal new static WorkItemTypeCollectionComparer Default => Nested.Instance;

        public override bool Equals(IWorkItemTypeCollection? x, IWorkItemTypeCollection? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            // Check if both collections contain the same work item types by name.
            // We need symmetric comparison: all items in x must exist in y and vice versa.
            // Note: We compare by work item type name, which is the unique identifier.

            // First, check that all types in x exist and match in y
            foreach (var wit in x)
            {
                var witName = wit.Name;
                if (witName == null || !y.Contains(witName))
                {
                    return false;
                }

                var tw = y[witName];
                if (!WorkItemTypeComparer.Default.Equals(wit, tw))
                {
                    return false;
                }
            }

            // Then, check that all types in y exist in x (they already matched above if they exist)
            foreach (var wit in y)
            {
                var witName = wit.Name;
                if (witName == null || !x.Contains(witName))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode(IWorkItemTypeCollection obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            // IMPORTANT: The collection must be in the same order to produce the same hash
            var hash = 27;
            foreach (var wit in obj.OrderBy(p => p.Name)) hash = (13 * hash) ^ wit.GetHashCode();
            return hash;
        }

        // ReSharper disable ClassNeverInstantiated.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Nested
        // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemTypeCollectionComparer Instance = new WorkItemTypeCollectionComparer();

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