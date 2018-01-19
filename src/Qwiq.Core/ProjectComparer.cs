using System;

namespace Microsoft.Qwiq
{
    internal class ProjectComparer : GenericComparer<IProject>
    {
        internal new static ProjectComparer Default => Nested.Instance;

        public override bool Equals(IProject x, IProject y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase)
                   && x.Guid.Equals(y.Guid)
                   && Equals(x.AreaRootNodes, y.AreaRootNodes)
                   && Equals(x.IterationRootNodes, y.IterationRootNodes)
                   && WorkItemTypeCollectionComparer.Default.Equals(x.WorkItemTypes, y.WorkItemTypes);
        }

        public override int GetHashCode(IProject obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;
                hash = (13 * hash) ^ obj.Guid.GetHashCode();
                hash = (13 * hash) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (13 * hash) ^ Comparer.WorkItemClassificationNodeCollection.GetHashCode(obj.AreaRootNodes);
                hash = (13 * hash) ^ Comparer.WorkItemClassificationNodeCollection.GetHashCode(obj.IterationRootNodes);
                hash = (13 * hash) ^ Comparer.WorkItemTypeCollection.GetHashCode(obj.WorkItemTypes);

                return hash;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly ProjectComparer Instance = new ProjectComparer();
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