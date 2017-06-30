using System.Linq;

namespace Microsoft.Qwiq
{
    internal class WorkItemTypeCollectionComparer : GenericComparer<IWorkItemTypeCollection>
    {
        internal new static WorkItemTypeCollectionComparer Default => Nested.Instance;

        public override bool Equals(IWorkItemTypeCollection x, IWorkItemTypeCollection y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            if (x.Count != y.Count) return false;

            var expected = x.ToList();
            var source = y.ToList();
            foreach (var wit in expected)
            {
                if (!y.Contains(wit.Name)) return false;
                var tw = y[wit.Name];
                if (!WorkItemTypeComparer.Default.Equals(wit, tw)) return false;

                // Removes the first occurrence, so if there are duplicates we'll still get a valid mismatch
                source.Remove(wit);
            }

            // If there are any items left then fail
            if (source.Any()) return false;

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