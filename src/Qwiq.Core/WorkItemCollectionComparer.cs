using System.Linq;

namespace Microsoft.Qwiq
{
    internal class WorkItemCollectionComparer : GenericComparer<IWorkItemCollection>
    {
        internal new static WorkItemCollectionComparer Default => Nested.Instance;

        private WorkItemCollectionComparer()
        {
        }

        /// <inheritdoc />
        public override bool Equals(IWorkItemCollection x, IWorkItemCollection y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            var source = y.ToList();
            var expected = x.ToList();

            if (source.Count != expected.Count) return false;

            foreach (var item in expected)
            {
                if (!y.Contains(item.Id)) return false;

                var tf = y.GetById(item.Id);
                if (!Comparer.WorkItem.Equals(item, tf)) return false;

                // Removes the first occurrence, so if there are duplicates we'll still get a valid mismatch
                source.Remove(item);
            }

            // If there are any items left then fail
            if (source.Any()) return false;

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode(IWorkItemCollection obj)
        {
            if (ReferenceEquals(obj, null)) return 0;
            var hash = 27;
            foreach (var item in obj.OrderBy(p => p.Id))
            {
                var itemHash = item.GetHashCode();
                hash = (13 * hash) ^ itemHash;
            }
            return hash;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemCollectionComparer Instance =
                    new WorkItemCollectionComparer();

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