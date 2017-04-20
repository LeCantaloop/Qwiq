using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class WorkItemCollection : ReadOnlyCollectionWithId<IWorkItem, int>, IWorkItemCollection
    {
        public WorkItemCollection(IEnumerable<IWorkItem> workItems)
            :base(workItems)
        {
        }

        /// <inheritdoc />
        public bool Equals(IWorkItemCollection other)
        {
            return Comparer.WorkItemCollection.Equals(this, other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
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
