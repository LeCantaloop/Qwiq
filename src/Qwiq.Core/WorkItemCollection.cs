using JetBrains.Annotations;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class WorkItemCollection : ReadOnlyObjectWithIdCollection<IWorkItem, int>, IWorkItemCollection
    {
        internal WorkItemCollection([CanBeNull] List<IWorkItem> workItems)
            : base(workItems)
        {
        }

        internal WorkItemCollection([CanBeNull] IEnumerable<IWorkItem> workItems)
            : base(workItems)
        {
        }

        /// <inheritdoc />
        public bool Equals([CanBeNull] IWorkItemCollection other)
        {
            return Comparer.WorkItemCollection.Equals(this, other);
        }

        /// <inheritdoc />
        public override bool Equals([CanBeNull] object obj)
        {
            return Comparer.WorkItemCollection.Equals(this, obj as IWorkItemCollection);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Comparer.WorkItemCollection.GetHashCode(this);
        }
    }
}