using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class WorkItemTypeCollection : ReadOnlyList<IWorkItemType>,
                                          IWorkItemTypeCollection,
                                          IEquatable<IWorkItemTypeCollection>
    {
        internal WorkItemTypeCollection()
            : this(null)
        {
        }

        internal WorkItemTypeCollection(params IWorkItemType[] workItemTypes)
            : this(workItemTypes as IEnumerable<IWorkItemType>)
        {
        }

        internal WorkItemTypeCollection(IEnumerable<IWorkItemType> workItemTypes)
            : base(workItemTypes, type => type.Name)
        {
        }

        public bool Equals(IWorkItemTypeCollection other)
        {
            return WorkItemTypeCollectionComparer.Instance.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return WorkItemTypeCollectionComparer.Instance.Equals(this, obj as IWorkItemTypeCollection);
        }

        public override int GetHashCode()
        {
            return WorkItemTypeCollectionComparer.Instance.GetHashCode(this);
        }
    }
}