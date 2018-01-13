using System;
using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    internal class WorkItemClassificationNodeComparer<TId> : GenericComparer<IWorkItemClassificationNode<TId>>
    {
        internal static new readonly WorkItemClassificationNodeComparer<TId> Default = Nested.Instance;

        private WorkItemClassificationNodeComparer()
        {
        }

        public override bool Equals(IWorkItemClassificationNode<TId> x, IWorkItemClassificationNode<TId> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return GenericComparer<TId>.Default.Equals(x.Id, y.Id)
                   && x.Type == y.Type
                   && string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode([CanBeNull] IWorkItemClassificationNode<TId> obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            unchecked
            {
                var hash = 27;

                hash = (hash * 13) ^ GenericComparer<TId>.Default.GetHashCode(obj.Id);
                hash = (hash * 13) ^ (obj.Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name) : 0);
                hash = (hash * 13) ^ obj.Type.GetHashCode();

                return hash;
            }
        }

        private class Nested
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly WorkItemClassificationNodeComparer<TId> Instance = new WorkItemClassificationNodeComparer<TId>();

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